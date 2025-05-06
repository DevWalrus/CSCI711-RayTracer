using RayTracer.RayMath;
using RayTracer.Objects;
using RayTracer.Shaders;
using System.Globalization;

namespace RayTracer.Utils
{
    public static class ObjParser
    {
        /// <summary>
        /// Parses a Wavefront .obj file (ASCII only), triangulates faces,
        /// and returns a list of Triangle objects using materials from the supplied map.
        /// </summary>
        public static List<Triangle> ParseObjFile(
            string filePath,
            Material defaultMaterial)
        {
            var vertices = new List<Point>();
            var normals = new List<MyVector>();
            var uvs = new List<(float U, float V)>();
            var triangles = new List<Triangle>();
            var culture = CultureInfo.InvariantCulture;
            var materialMap = new Dictionary<string, Material>();
            var dir = Path.GetDirectoryName(filePath) ?? string.Empty;

            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("mtllib "))
                {
                    materialMap = ParseMtlFile(Path.Join(dir, line.Split(' ')[1]), defaultMaterial);
                    break;
                }
            }

            // start with default
            Material currentMaterial = defaultMaterial;
            bool hasNormals = false;

            foreach (var raw in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                var parts = raw.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                switch (parts[0])
                {
                    case "v":
                        // vertex
                        float vx = float.Parse(parts[1], culture);
                        float vy = float.Parse(parts[2], culture);
                        float vz = float.Parse(parts[3], culture);
                        vertices.Add(new Point(vx, vy, vz));
                        break;

                    case "vn":
                        // normal
                        float nx = float.Parse(parts[1], culture);
                        float ny = float.Parse(parts[2], culture);
                        float nz = float.Parse(parts[3], culture);
                        normals.Add(new MyVector(nx, ny, nz).Normalize());
                        hasNormals = true;
                        break;

                    case "vt":
                        // texture coordinate
                        float u = float.Parse(parts[1], culture);
                        float v = float.Parse(parts[2], culture);
                        uvs.Add((u, v));
                        break;

                    case "usemtl":
                        // switch material if in map
                        var name = parts.Length > 1 ? parts[1] : null;
                        if (name != null && materialMap.TryGetValue(name, out var mat))
                            currentMaterial = mat;
                        else
                            currentMaterial = defaultMaterial;
                        break;

                    case "f":
                        // face: v/vt/vn
                        var idxs = parts.Skip(1)
                            .Select(p => {
                                var c = p.Split('/');
                                int vi = int.Parse(c[0]) - 1;
                                int ti = (c.Length > 1 && c[1] != "") ? int.Parse(c[1]) - 1 : -1;
                                int ni = (c.Length > 2 && c[2] != "") ? int.Parse(c[2]) - 1 : -1;
                                return (vi, ti, ni);
                            })
                            .ToList();

                        if (idxs.Count < 3)
                            break;

                        // triangulate fan
                        for (int i = 1; i < idxs.Count - 1; i++)
                        {
                            var (i0, ti0, n0) = idxs[0];
                            var (i1, ti1, n1) = idxs[i];
                            var (i2, ti2, n2) = idxs[i + 1];

                            var v0 = vertices[i0];
                            var v1 = vertices[i1];
                            var v2 = vertices[i2];

                            MyVector normal;
                            if (hasNormals && n0 >= 0 && n1 >= 0 && n2 >= 0)
                            {
                                // average normals
                                normal = (normals[n0] + normals[n1] + normals[n2]) / 3f;
                                normal = normal.Normalize();
                            }
                            else
                            {
                                normal = ComputeNormal(v0, v1, v2);
                            }

                            var uv0 = ti0 >= 0 ? uvs[ti0] : (0f, 0f);
                            var uv1 = ti1 >= 0 ? uvs[ti1] : (0f, 0f);
                            var uv2 = ti2 >= 0 ? uvs[ti2] : (0f, 0f);

                            triangles.Add(new Triangle(
                              [vertices[i0], vertices[i1], vertices[i2]],
                              [uv0, uv1, uv2],
                              normal,
                              currentMaterial));
                        }
                        break;
                }
            }

            return triangles;
        }

        private static MyVector ComputeNormal(Point v0, Point v1, Point v2)
        {
            var e1 = new MyVector(v0, v1);
            var e2 = new MyVector(v0, v2);
            return e1.Cross(e2).Normalize();
        }

        private static Dictionary<string, Material> ParseMtlFile(string mtlPath, Material defaultMaterial)
        {
            var map = new Dictionary<string, Material>(StringComparer.OrdinalIgnoreCase);
            string? name = null;
            var ka = new Color(1, 1, 1);
            var kd = new Color(1, 1, 1);
            var ks = new Color(1, 1, 1);
            double ns = 1, ni = 1, d = 1;
            string? tex = null;
            var dir = Path.GetDirectoryName(mtlPath) ?? string.Empty;

            foreach (var raw in File.ReadLines(mtlPath))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#")) continue;
                var parts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                switch (parts[0].ToLowerInvariant())
                {
                    case "newmtl":
                        if (name != null)
                            map[name] = CreateMaterial(ka, kd, ks, ns, ni, d, tex, defaultMaterial);
                        name = parts[1];
                        ka = kd = ks = new Color(1, 1, 1);
                        ns = ni = d = 1;
                        tex = null;
                        break;
                    case "ka":
                        var ac = parts[1].Split(' ');
                        ka = new Color(float.Parse(ac[0]), float.Parse(ac[1]), float.Parse(ac[2]));
                        break;
                    case "kd":
                        var dc = parts[1].Split(' ');
                        kd = new Color(float.Parse(dc[0]), float.Parse(dc[1]), float.Parse(dc[2]));
                        break;
                    case "ks":
                        var sc = parts[1].Split(' ');
                        ks = new Color(float.Parse(sc[0]), float.Parse(sc[1]), float.Parse(sc[2]));
                        break;
                    case "ns": ns = double.Parse(parts[1]); break;
                    case "ni": ni = double.Parse(parts[1]); break;
                    case "d": d = double.Parse(parts[1]); break;
                    case "map_kd":
                        var rawPath = parts[1].Trim('"');
                        var fileName = Path.GetFileName(rawPath);
                        tex = Path.Combine(dir, fileName);
                        break;
                }
            }
            if (name != null)
                map[name] = CreateMaterial(ka, kd, ks, ns, ni, d, tex, defaultMaterial);
            return map;
        }

        private static Material CreateMaterial(
            Color ka, Color kd, Color ks, double ns,
            double ni, double d, string? texPath,
            Material defaultMat)
        {
            Material baseShader = new ColorShader(kd);
            if (!string.IsNullOrEmpty(texPath))
                baseShader = new ImageShader(texPath, scale: 1);
            double reflectivity = ks.Luminance();
            double transparency = 1 - d;
            return new PhongShader(
                (ka.R + ka.G + ka.B) / 3,
                (kd.R + kd.G + kd.B) / 3,
                reflectivity,
                ns,
                baseShader,
                reflectivity,
                transparency,
                ni
            );
        }

    }
}

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
        /// and returns a list of Triangle objects all using the given defaultMaterial.
        /// Later you can hook up `usemtl` to swap materials per‐object.
        /// </summary>
        public static List<Triangle> ParseObjFile(string filePath, Material defaultMaterial)
        {
            var vertices = new List<Point>();
            var normals = new List<MyVector>();
            var triangles = new List<Triangle>();
            var culture = CultureInfo.InvariantCulture;

            // currentMaterial = defaultMaterial for now,
            // in future you can switch it when you see "usemtl"
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

                    case "usemtl":
                        // TODO: in the future load or switch currentMaterial by name
                        // string mtlName = parts[1];
                        break;

                    case "f":
                        // face: could be "f v1 v2 v3 ..." or "f v1/vt1/vn1 ..." etc.
                        var idxs = parts
                            .Skip(1)
                            .Select(p =>
                            {
                                var comps = p.Split('/');
                                int vi = int.Parse(comps[0], culture) - 1;
                                int ni = (comps.Length >= 3 && !string.IsNullOrEmpty(comps[2]))
                                         ? int.Parse(comps[2], culture) - 1
                                         : -1;
                                return (vi, ni);
                            })
                            .ToList();

                        if (idxs.Count < 3)
                            break;

                        // triangulate fan: (0, i, i+1)
                        for (int i = 1; i < idxs.Count - 1; i++)
                        {
                            var (i0, n0) = idxs[0];
                            var (i1, n1) = idxs[i];
                            var (i2, n2) = idxs[i + 1];

                            var v0 = vertices[i0];
                            var v1 = vertices[i1];
                            var v2 = vertices[i2];

                            MyVector normal;
                            if (hasNormals && n0 >= 0 && n1 >= 0 && n2 >= 0)
                            {
                                // average the per‐vertex normals
                                normal = (normals[n0] + normals[n1] + normals[n2]) / 3f;
                                normal = normal.Normalize();
                            }
                            else
                            {
                                normal = ComputeNormal(v0, v1, v2);
                            }

                            triangles.Add(new Triangle([v0, v1, v2], normal, currentMaterial));
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
    }
}

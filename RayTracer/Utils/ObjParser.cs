using RayTracer.RayMath;
using RayTracer.Objects;
using RayTracer.Shaders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Utils
{
    public static class PlyParser
    {
        /// <summary>
        /// Parses a PLY file (assumed to be in ASCII format) and returns a list of triangles.
        /// Each triangle will use the given material.
        /// </summary>
        /// <param name="filePath">Path to the PLY file.</param>
        /// <param name="material">The material to apply to every triangle.</param>
        /// <returns>List of Triangle objects.</returns>
        public static List<Triangle> ParsePlyFile(string filePath, Material material)
        {
            var triangles = new List<Triangle>();
            var vertices = new List<Point>();

            int vertexCount = 0;
            int faceCount = 0;
            bool headerEnded = false;

            // Use invariant culture for parsing floats.
            var culture = CultureInfo.InvariantCulture;

            using (var reader = new StreamReader(filePath))
            {
                string? line;
                // Process header
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("element vertex"))
                    {
                        // Example: "element vertex 35947"
                        var parts = line.Split(' ');
                        vertexCount = int.Parse(parts[2]);
                    }
                    else if (line.StartsWith("element face"))
                    {
                        // Example: "element face 69451"
                        var parts = line.Split(' ');
                        faceCount = int.Parse(parts[2]);
                    }
                    else if (line.StartsWith("end_header"))
                    {
                        headerEnded = true;
                        break;
                    }
                }

                if (!headerEnded)
                    throw new Exception("PLY header did not end properly.");

                // Parse vertex data.
                for (int i = 0; i < vertexCount; i++)
                {
                    line = reader.ReadLine();
                    if (line == null)
                        throw new Exception("Unexpected end of file while reading vertices.");

                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    // Use only x, y, z (first three values)
                    float x = float.Parse(parts[0], culture);
                    float y = float.Parse(parts[1], culture);
                    float z = float.Parse(parts[2], culture);
                    vertices.Add(new Point(x, y, z));
                }

                // Parse face data.
                for (int i = 0; i < faceCount; i++)
                {
                    line = reader.ReadLine();
                    if (line == null)
                        throw new Exception("Unexpected end of file while reading faces.");

                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    int count = int.Parse(parts[0], culture);
                    if (count < 3)
                        continue; // Not a valid face

                    // For a face with more than 3 vertices, triangulate using a triangle fan.
                    // First, parse all the vertex indices.
                    var indices = new List<int>();
                    for (int j = 1; j <= count; j++)
                    {
                        indices.Add(int.Parse(parts[j], culture));
                    }

                    // Create triangles. If the face is already a triangle, this loop will run once.
                    for (int j = 1; j < indices.Count - 1; j++)
                    {
                        var v0 = vertices[indices[0]];
                        var v1 = vertices[indices[j]];
                        var v2 = vertices[indices[j + 1]];
                        MyVector normal = ComputeNormal(v0, v1, v2);
                        triangles.Add(new Triangle([v0, v1, v2], normal, material));
                    }
                }
            }
            return triangles;
        }

        private static MyVector ComputeNormal(Point v0, Point v1, Point v2)
        {
            MyVector edge1 = new MyVector(v0, v1);
            MyVector edge2 = new MyVector(v0, v2);
            return edge1.Cross(edge2).Normalize();
        }

        public static void WritePlyFile(string filePath, MeshObject mesh)
        {
            // Dictionary to maintain unique vertices. 
            // If your Point class implements GetHashCode() and Equals() correctly, you can use Dictionary<Point, int> instead.
            Dictionary<string, int> vertexToIndex = new Dictionary<string, int>();
            List<Point> vertices = new List<Point>();
            List<int[]> faces = new List<int[]>();

            // Iterate over every triangle in the mesh.
            foreach (var triangle in mesh.Triangles)
            {
                // Assume each triangle has exactly 3 vertices available via a property "Vertices".
                int[] faceIndices = new int[3];
                for (int i = 0; i < 3; i++)
                {
                    Point vertex = triangle[i];
                    // Build a key for the vertex using invariant culture formatting.
                    string key = $"{vertex.X.ToString(CultureInfo.InvariantCulture)}_" +
                                 $"{vertex.Y.ToString(CultureInfo.InvariantCulture)}_" +
                                 $"{vertex.Z.ToString(CultureInfo.InvariantCulture)}";
                    if (!vertexToIndex.ContainsKey(key))
                    {
                        vertexToIndex[key] = vertices.Count;
                        vertices.Add(vertex);
                    }
                    faceIndices[i] = vertexToIndex[key];
                }
                faces.Add(faceIndices);
            }

            // Write to file in ASCII PLY format.
            using (var writer = new StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("ply");
                writer.WriteLine("format ascii 1.0");
                writer.WriteLine($"element vertex {vertices.Count}");
                writer.WriteLine("property float x");
                writer.WriteLine("property float y");
                writer.WriteLine("property float z");
                writer.WriteLine($"element face {faces.Count}");
                writer.WriteLine("property list uchar int vertex_indices");
                writer.WriteLine("end_header");

                // Write vertex data
                foreach (var v in vertices)
                {
                    writer.WriteLine($"{v.X.ToString(CultureInfo.InvariantCulture)} " +
                                       $"{v.Y.ToString(CultureInfo.InvariantCulture)} " +
                                       $"{v.Z.ToString(CultureInfo.InvariantCulture)}");
                }

                // Write face data (each face is a triangle, so it starts with a "3")
                foreach (var face in faces)
                {
                    writer.WriteLine($"3 {face[0]} {face[1]} {face[2]}");
                }
            }
        }
    }
}

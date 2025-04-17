using MathNet.Numerics.LinearAlgebra;
using RayTracer.RayMath;
using RayTracer.Shaders;
using RayTracer.Utils;
using System.Diagnostics;

namespace RayTracer.Objects
{
    public class MeshObject : RenderableObject
    {
        private readonly List<Triangle> _triangles;
        private KdTreeNode _kdTree;

        public List<Triangle> Triangles { get => _triangles; }

        public MeshObject(List<Triangle> triangles, Material material) : base(material)
        {
            _triangles = triangles;
            Stopwatch sw = Stopwatch.StartNew();
            _kdTree = new KdTreeNode([.. triangles]);
            sw.Stop();
            Console.WriteLine($"Object KD Tree Generation completed in: {sw.ElapsedMilliseconds} ms");

            Center = Point.Center([_kdTree.BoundingBox.Min, _kdTree.BoundingBox.Max]);
            BBMin = _kdTree.BoundingBox.Min;
            BBMax = _kdTree.BoundingBox.Max;
        }

        /// <inheritdoc/>
        public override Intersection? Intersect(Ray ray, double minIntersection = 0)
        {
            var hit = _kdTree.Traverse(ray, double.MaxValue);
            if (hit != null && hit.Omega >= minIntersection)
            {
                return hit;
            }
            return null;
        }

        /// <inheritdoc/>
        public override void Transform(Matrix<double> transformationMatrix)
        {
            foreach (var tri in _triangles)
            {
                tri.Transform(transformationMatrix);
            }
            _kdTree = new KdTreeNode([.. _triangles]);

            Center = Point.Center([_kdTree.BoundingBox.Min, _kdTree.BoundingBox.Max]);
            BBMin = _kdTree.BoundingBox.Min;
            BBMax = _kdTree.BoundingBox.Max;
        }

        public void Scale(double scaleFactor)
        {
            // Assume the mesh's center is stored in the 'Center' property.
            // This center should reflect the geometric center of your mesh.
            var center = this.Center;

            // Create a 4x4 translation matrix to move the mesh so that its center is at the origin.
            var translateToOrigin = Matrix<double>.Build.DenseIdentity(4);
            translateToOrigin[0, 3] = -center.X;
            translateToOrigin[1, 3] = -center.Y;
            translateToOrigin[2, 3] = -center.Z;

            // Create a 4x4 scaling matrix.
            // The homogeneous coordinate (bottom-right corner) remains 1.
            var scalingMatrix = Matrix<double>.Build.DenseIdentity(4);
            scalingMatrix[0, 0] = scaleFactor;
            scalingMatrix[1, 1] = scaleFactor;
            scalingMatrix[2, 2] = scaleFactor;

            // Create a 4x4 translation matrix to move the mesh back to its original position.
            var translateBack = Matrix<double>.Build.DenseIdentity(4);
            translateBack[0, 3] = center.X;
            translateBack[1, 3] = center.Y;
            translateBack[2, 3] = center.Z;

            // Combine the matrices:
            // Note: matrix multiplication order is important. We translate back, then scale, then translate to origin.
            var transformationMatrix = translateBack * scalingMatrix * translateToOrigin;

            // Apply the transformation to every triangle in the mesh.
            this.Transform(transformationMatrix);
        }
    }
}

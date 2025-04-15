using MathNet.Numerics.LinearAlgebra;
using RayTracer.RayMath;
using RayTracer.Shaders;

namespace RayTracer.Objects
{
    public class MeshObject : RenderableObject
    {
        private readonly List<Triangle> _triangles;
        private KdTreeNode _kdTree;

        public List<Triangle> Triangles { get => _triangles; }

        public MeshObject(string filePath, Material material) : base(material)
        {
            _triangles = PlyParser.ParsePlyFile(filePath, material);

            _kdTree = new KdTreeNode([.. _triangles]);

            Center = Point.Center([_kdTree.BoundingBox.Min, _kdTree.BoundingBox.Max]); ;
            BBMin = _kdTree.BoundingBox.Min;
            BBMax = _kdTree.BoundingBox.Max;
        }

        public MeshObject(List<Triangle> triangles, Material material) : base(material)
        {
            _triangles = triangles;
            _kdTree = new KdTreeNode([.. triangles]);

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
    }
}

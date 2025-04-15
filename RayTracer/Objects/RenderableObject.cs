using MathNet.Numerics.LinearAlgebra;
using RayTracer.Shaders;

namespace RayTracer.Objects
{
    public abstract class RenderableObject
    {
        public readonly Material Material;
        public Point Center;
        public Point BBMin;
        public Point BBMax;

        public RenderableObject(
            Material material
        ) : this(
            material,
            new Point(0, 0, 0),
            new Point(double.MinValue, double.MinValue, double.MinValue),
            new Point(double.MaxValue, double.MaxValue, double.MaxValue)
        ) { }

        public RenderableObject(Material material, Point center, Point bbMin, Point bbMax)
        {
            Material = material;
            Center = center;
            BBMin = bbMin;
            BBMax = bbMax;
        }

        public abstract Intersection? Intersect(Ray ray, double minIntersection = 0);
        public abstract void Transform(Matrix<double> m);
    }
}

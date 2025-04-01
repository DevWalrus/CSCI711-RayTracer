using MathNet.Numerics.LinearAlgebra;
using RayTracer.Shaders;

namespace RayTracer.Objects
{
    public abstract class RenderableObject
    {
        public Material material;

        public RenderableObject(Material material)
        {
            this.material = material;
        }

        public abstract Intersection? Intersect(Ray ray, double minIntersection = 0);
        public abstract void Transform(Matrix<double> m);
    }
}

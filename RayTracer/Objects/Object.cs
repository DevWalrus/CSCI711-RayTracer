using MathNet.Numerics.LinearAlgebra;

namespace RayTracer.Objects
{
    public abstract class RenderableObject
    {
        public Color material;

        public RenderableObject(Color material)
        {
            this.material = material;
        }

        public abstract Interseciton? Intersect(Ray ray, double minIntersection = 0);
        public abstract void Transform(Matrix<double> m);
    }
}

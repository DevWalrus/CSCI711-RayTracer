using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Spatial.Euclidean;

namespace RayTracer.RayMath
{
    public class Ray
    {
        public Point Origin;
        public MyVector Direction;

        public Ray(Point origin, MyVector direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public override string ToString()
        {
            return $"Origin: {Origin}, Direction: {Direction}";
        }
    }
}

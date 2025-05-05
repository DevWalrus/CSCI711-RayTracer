using MathNet.Numerics.LinearAlgebra;

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

        public Ray Transform(Matrix<double> M)
        {
            // build the 4D homogeneous vectors
            var o4 = Vector<double>.Build.Dense([ Origin.X, Origin.Y, Origin.Z, 1.0 ]);
            var d4 = Vector<double>.Build.Dense([ Direction.X, Direction.Y, Direction.Z, 0.0 ]);

            var oT = M * o4;
            var dT = M * d4;

            return new Ray(
                new Point(oT[0], oT[1], oT[2]),
                new MyVector(dT[0], dT[1], dT[2])
            );
        }

        public Point At(double t)
        {
            return new Point(
                Origin.X + t * Direction.X,
                Origin.Y + t * Direction.Y,
                Origin.Z + t * Direction.Z
            );
        }

        public override string ToString()
        {
            return $"Origin: {Origin}, Direction: {Direction}";
        }
    }
}

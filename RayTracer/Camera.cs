using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RayTracer
{
    internal class Camera
    {
        public Point Position;
        public Point Lookat;
        public MyVector Up;
        public Matrix<double> ViewMatrix;

        public Camera(Point position, Point lookAt, MyVector up)
        {
            Position = position;
            Lookat = lookAt;
            Up = up.Normalize();
            ViewMatrix =  ComputeViewMatrix();
        }

        private Matrix<double> ComputeViewMatrix()
        {
            var n = Position.Subtract(Lookat).Normalize();
            var u = Up.Cross(n).Normalize();
            var v = n.Cross(u);

            var posVect = new MyVector(Position.X, Position.Y, Position.Z);

            return DenseMatrix.OfArray(new double[,]
            {
                { u.X(), u.Y(), u.Z(), -u.Dot(posVect) },
                { v.X(), v.Y(), v.Z(), -v.Dot(posVect) },
                { n.X(), n.Y(), n.Z(), -n.Dot(posVect) },
                {     0,     0,     0,               1 }
            });
        }

        public void render(World world)
        {

        }
    }
}

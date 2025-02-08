using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Drawing;

namespace RayTracer
{
    internal class Camera
    {
        public Point Origin;
        public Point Lookat;
        public MyVector Up;
        public Matrix<double> ViewMatrix;

        private double _focalDistance = -1;

        private int _imageWidth = 800;
        private int _imageHeight = 800;

        private double _filmPlaneWidth = 1;
        private double _filmPlaneHeight = 1;
        private Point _cameraCoordsOrigin = new Point(0, 0, 0);

        public Camera(Point position, Point lookAt, MyVector up)
        {
            Origin = position;
            Lookat = lookAt;
            Up = up.Normalize();
            ViewMatrix =  ComputeViewMatrix();
        }

        private Matrix<double> ComputeViewMatrix()
        {
            var n = Origin.Subtract(Lookat).Normalize();
            var u = Up.Cross(n).Normalize();
            var v = n.Cross(u);

            var posVect = new MyVector(Origin.X, Origin.Y, Origin.Z);

            return DenseMatrix.OfArray(new double[,]
            {
                { u.X, u.Y, u.Z, -u.Dot(posVect) },
                { v.X, v.Y, v.Z, -v.Dot(posVect) },
                { n.X, n.Y, n.Z, -n.Dot(posVect) },
                {   0,   0,   0,               1 }
            });
        }

        public Bitmap render(World world)
        {
            var bitmap = new Bitmap(_imageWidth, _imageHeight);

            world.TransformAllObjects(ViewMatrix);

            var pixelWidth = _filmPlaneWidth / _imageWidth;
            var pixelHeight = _filmPlaneHeight / _imageHeight;

            var topLeftPixel = new Point(
                -(_filmPlaneWidth / 2) + (pixelWidth / 2),
                (_filmPlaneHeight / 2) - (pixelHeight / 2),
                _focalDistance);

            var lastPoint = topLeftPixel;

            int x = 0, y = 0;

            do
            {
                do
                {

                    var rayToPixel = new Ray(_cameraCoordsOrigin, lastPoint.Subtract(_cameraCoordsOrigin).Normalize());

                    var intersection = world.Spawn(rayToPixel);

                    if (intersection != null)
                    {
                        bitmap.SetPixel(x, y, intersection.Material.ToSystemColor());
                    }

                    lastPoint = new Point(lastPoint.X + (pixelWidth), lastPoint.Y, lastPoint.Z);

                    x++;

                } while (lastPoint.X < (_filmPlaneWidth / 2));

                lastPoint = new Point(topLeftPixel.X, lastPoint.Y - (pixelWidth), lastPoint.Z);

                x = 0;
                y++;

            } while (lastPoint.Y > -(_filmPlaneHeight / 2));


            return bitmap;
        }
    }
}

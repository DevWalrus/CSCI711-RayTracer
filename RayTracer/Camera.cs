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
                 _focalDistance
            );

            var currentPixel = topLeftPixel.Copy();

            for (int y = 0; y < _imageHeight; y++)
            {
                currentPixel.X = topLeftPixel.X;

                for (int x = 0; x < _imageWidth; x++)
                {
                    var direction = currentPixel.Subtract(_cameraCoordsOrigin).Normalize();
                    var rayToPixel = new Ray(_cameraCoordsOrigin, direction);
                    var intersection = world.Spawn(rayToPixel);

                    if (intersection != null)
                    {
                        var viewDir = new MyVector(-direction.X, -direction.Y, -direction.Z).Normalize();

                        var shadedColor = intersection.Material.Shade(
                            intersection.Position,   // Intersection point
                            intersection.Normal,     // Surface normal
                            viewDir,                 // Direction from point -> camera
                            world                    // So it can access lights, etc.
                        );

                        bitmap.SetPixel(x, y, shadedColor.ToSystemColor());
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, System.Drawing.Color.Black);
                    }

                    currentPixel.X += pixelWidth;
                }

                currentPixel.Y -= pixelHeight;
            }

            return bitmap;
        }

    }
}

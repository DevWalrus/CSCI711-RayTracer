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

        private double _focalDistance = -1.0;

        private int _imageWidth = 800;
        private int _imageHeight = 800;

        private double _filmPlaneWidth = 1;
        private double _filmPlaneHeight = 1;
        private Point _cameraCoordsOrigin = new Point(0, 0, 0);

        private double _apertureRadius = 0.01;
        private int _samplesPerPixel = 25;

        public Camera(Point position, Point lookAt, MyVector up)
        {
            Origin = position;
            Lookat = lookAt;
            Up = up.Normalize();
            ViewMatrix = ComputeViewMatrix();
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

        private (double, double) RandomInUnitDisk(Random rand)
        {
            double x, y;
            do
            {
                x = rand.NextDouble() * 2 - 1;
                y = rand.NextDouble() * 2 - 1;
            } while (x * x + y * y >= 1);
            return (x, y);
        }

        public void SetAperture(double fStop)
        {
            // Here focalLength is in the same unit as your aperture radius.
            // For a realistic mapping, choose focalLength such that f/1.4 yields ~0.01.
            double focalLength = 0.028;
            // Compute the aperture radius using the physical relationship.
            _apertureRadius = focalLength / (2 * fStop);
        }

        public void SetPinhole()
        {
            _apertureRadius = 0;
            _samplesPerPixel = 1;
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

            Random rand = new Random();

            for (int y = 0; y < _imageHeight; y++)
            {
                currentPixel.X = topLeftPixel.X;

                for (int x = 0; x < _imageWidth; x++)
                {
                    Color accumulatedColor = new Color(0, 0, 0);
                    //Console.WriteLine($"{-halfFilmWidth + pixelWidth * (x + 0.5)}, {halfFilmHeight - pixelHeight * (y + 0.5)}");
                    for (int sample = 0; sample < _samplesPerPixel; sample++)
                    {
                        MyVector idealRayDir = currentPixel.Subtract(_cameraCoordsOrigin).Normalize();

                        Point focalPoint = new Point(
                            _cameraCoordsOrigin.X + idealRayDir.X * Math.Abs(_focalDistance),
                            _cameraCoordsOrigin.Y + idealRayDir.Y * Math.Abs(_focalDistance),
                            _cameraCoordsOrigin.Z + idealRayDir.Z * Math.Abs(_focalDistance)
                        );

                        (double diskX, double diskY) = RandomInUnitDisk(rand);

                        diskX *= _apertureRadius;
                        diskY *= _apertureRadius;
                        Point lensOrigin = new Point(
                            _cameraCoordsOrigin.X + diskX,
                            _cameraCoordsOrigin.Y + diskY,
                            _cameraCoordsOrigin.Z
                        );

                        MyVector newRayDir = focalPoint.Subtract(lensOrigin).Normalize();
                        Ray ray = new Ray(lensOrigin, newRayDir);

                        var intersection = world.Spawn(ray);
                        if (intersection != null)
                        {
                            var viewDir = new MyVector(-newRayDir.X, -newRayDir.Y, -newRayDir.Z).Normalize();
                            Color shadedColor = intersection.Material.Shade(
                                intersection.Position,   // Intersection point
                                intersection.Normal,
                                viewDir,
                                world
                            );
                            accumulatedColor = accumulatedColor + shadedColor;
                        }
                        else
                        {
                            accumulatedColor = accumulatedColor + new Color(0, 0, 0);
                        }
                    }

                    currentPixel.X += pixelWidth;

                    Color finalColor = accumulatedColor * (1.0 / _samplesPerPixel);
                    bitmap.SetPixel(x, y, finalColor.ToSystemColor());
                }

                currentPixel.Y -= pixelHeight;
            }

            return bitmap;
        }
    }
}

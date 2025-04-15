#pragma warning disable CA1416
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using RayTracer.Shaders;
using System.Drawing;
using System.Numerics;

namespace RayTracer
{
    internal class Camera
    {
        private const int MAX_DEPTH = 5;

        public Point Origin;
        public Point Lookat;
        public MyVector Up;
        public Matrix<double> ViewMatrix;
        public Matrix<double> CamToWorld;

        private double _focalDistance = -1.0;

        private int _imageWidth = 800;
        private int _imageHeight = 800;

        private double _filmPlaneWidth = 1;
        private double _filmPlaneHeight = 1;
        private Point _cameraCoordsOrigin = new Point(0, 0, 0);

        private double _apertureRadius = 0.01;
        private int _samplesPerPixel = 25;

        private bool _parallel;

        private static Random _rand = new Random();

        public Camera(Point position, Point lookAt, MyVector up, bool parallel)
        {
            Origin = position;
            Lookat = lookAt;
            Up = up.Normalize();
            ViewMatrix = ComputeViewMatrix();
            CamToWorld = ViewMatrix.Inverse();
            _parallel = parallel;
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

        private Color TraceRay(Ray ray, World world, int depth)
        {
            if (depth >= MAX_DEPTH)
                return world.BackgroundColor;

            var intersection = world.Spawn(ray);
            if (intersection == null)
                return world.BackgroundColor;

            var viewDir = new MyVector(-ray.Direction.X, -ray.Direction.Y, -ray.Direction.Z).Normalize();
            var localPoint = intersection.Position.Copy();
            localPoint.Transform(CamToWorld);

            var shadingInfo = new ShadingContext
            {
                WorldPosition = intersection.Position,
                LocalPosition = localPoint,
                Normal = intersection.Normal,
                ViewDirection = viewDir
            };

            var localColor = intersection.Material.Shade(shadingInfo, world);

            var kr = intersection.Material.Reflectivity;
            var kt = intersection.Material.Transparency;

            var reflectionColor = new Color(0, 0, 0);
            var refractionColor = new Color(0, 0, 0);

            if (kr > 0.0 && depth < MAX_DEPTH)
            {
                var reflectDir = ray.Direction.Normalize().Reflect(intersection.Normal);
                var reflectOrigin = new Point(
                    intersection.Position.X + reflectDir.X * 1e-6,
                    intersection.Position.Y + reflectDir.Y * 1e-6,
                    intersection.Position.Z + reflectDir.Z * 1e-6
                );
                var reflectionRay = new Ray(reflectOrigin, reflectDir);
                reflectionColor = TraceRay(reflectionRay, world, depth + 1);
            }

            if (kt > 0.0 && depth < MAX_DEPTH)
            {
                var incident = ray.Direction.Normalize();
                bool entering = incident.Dot(intersection.Normal) < 0;
                var normal = entering ? intersection.Normal : intersection.Normal * -1;
                var n1 = entering ? 1.0 : intersection.Material.IndexOfRefraction;
                var n2 = entering ? intersection.Material.IndexOfRefraction : 1.0;
                var eta = n1 / n2;
                var cosThetaI = incident.Dot(normal) * -1;
                var k = 1.0f - eta * eta * (1.0f - cosThetaI * cosThetaI);
                if (k < 0.0f)
                {
                    var reflectDir = ray.Direction.Normalize().Reflect(intersection.Normal);
                    var reflectOrigin = new Point(
                        intersection.Position.X + reflectDir.X * 1e-6,
                        intersection.Position.Y + reflectDir.Y * 1e-6,
                        intersection.Position.Z + reflectDir.Z * 1e-6
                    );
                    var reflectionRay = new Ray(reflectOrigin, reflectDir);
                    refractionColor = TraceRay(reflectionRay, world, depth + 1);
                }
                else
                {
                    var refractDir = (eta * incident + (eta * cosThetaI - (float)Math.Sqrt(k)) * normal).Normalize();
                    var refractOrigin = new Point(
                        intersection.Position.X + refractDir.X * 1e-6,
                        intersection.Position.Y + refractDir.Y * 1e-6,
                        intersection.Position.Z + refractDir.Z * 1e-6
                    );
                    var refractionRay = new Ray(refractOrigin, refractDir);
                    refractionColor = TraceRay(refractionRay, world, depth + 1);
                }
            }

            var finalColor = ((1 - kt) * localColor) + (kr * reflectionColor) + (kt * refractionColor);
            return finalColor;
        }

        private (int x, int y, Color color) spawnOnePixel(
            Point topLeftPixel,
            int currentX,
            int currentY,
            double pixelWidth,
            double pixelHeight,
            World world
        )
        {
            // Compute the pixel center.
            double posX = topLeftPixel.X + currentX * pixelWidth;
            double posY = topLeftPixel.Y - currentY * pixelHeight;
            Point pixelCenter = new Point(posX, posY, _focalDistance);

            Color accumulatedColor = new Color(0, 0, 0);
            Random localRand = new Random(currentX + currentY + Environment.TickCount);

            // Loop over multiple samples per pixel.
            for (int sample = 0; sample < _samplesPerPixel; sample++)
            {
                // For depth of field: offset within the lens.
                (double diskX, double diskY) = RandomInUnitDisk(localRand);
                diskX *= _apertureRadius;
                diskY *= _apertureRadius;

                Point lensOrigin = new Point(
                    _cameraCoordsOrigin.X + diskX,
                    _cameraCoordsOrigin.Y + diskY,
                    _cameraCoordsOrigin.Z
                );

                MyVector idealRayDir = pixelCenter.Subtract(_cameraCoordsOrigin).Normalize();
                Point focalPoint = new Point(
                    _cameraCoordsOrigin.X + idealRayDir.X * Math.Abs(_focalDistance),
                    _cameraCoordsOrigin.Y + idealRayDir.Y * Math.Abs(_focalDistance),
                    _cameraCoordsOrigin.Z + idealRayDir.Z * Math.Abs(_focalDistance)
                );

                MyVector newRayDir = focalPoint.Subtract(lensOrigin).Normalize();
                Ray ray = new Ray(lensOrigin, newRayDir);

                // Use the recursive trace function to get the sample color.
                Color sampleColor = TraceRay(ray, world, 0);
                accumulatedColor = accumulatedColor + sampleColor;
            }

            // Average the accumulated color.
            Color finalColor = accumulatedColor * (1.0 / _samplesPerPixel);
            return (currentX, currentY, finalColor);
        }


        public Bitmap render(World world)
        {
            world.TransformAllObjects(ViewMatrix);
            var pixelWidth = _filmPlaneWidth / _imageWidth;
            var pixelHeight = _filmPlaneHeight / _imageHeight;

            // Define the top-left pixel position (center of pixel)
            var topLeftPixel = new Point(
                -(_filmPlaneWidth / 2) + (pixelWidth / 2),
                 (_filmPlaneHeight / 2) - (pixelHeight / 2),
                 _focalDistance
            );

            if (_parallel)
            {
                // Create a list to hold tasks for each pixel.
                List<Task<(int x, int y, Color color)>> pixelTasks = new List<Task<(int, int, Color)>>();

                for (int y = 0; y < _imageHeight; y++)
                {
                    for (int x = 0; x < _imageWidth; x++)
                    {
                        int currentX = x;
                        int currentY = y;
                        pixelTasks.Add(Task.Run(() => spawnOnePixel(topLeftPixel, currentX, currentY, pixelWidth, pixelHeight, world)));
                    }
                }

                // Wait for all pixel tasks to complete.
                Task.WaitAll(pixelTasks.ToArray());

                // Create the final bitmap and set each pixel.
                var bitmap = new Bitmap(_imageWidth, _imageHeight);
                foreach (var task in pixelTasks)
                {
                    var result = task.Result;
                    bitmap.SetPixel(result.x, result.y, result.color.ToSystemColor());
                }

                return bitmap;
            } else
            {
                var bitmap = new Bitmap(_imageWidth, _imageHeight);
                for (int y = 0; y < _imageHeight; y++)
                {
                    for (int x = 0; x < _imageWidth; x++)
                    {
                        int currentX = x;
                        int currentY = y;
                        var result = spawnOnePixel(topLeftPixel, currentX, currentY, pixelWidth, pixelHeight, world);
                        bitmap.SetPixel(result.x, result.y, result.color.ToSystemColor());
                    }
                }
                return bitmap;
            }
        }
    }
}

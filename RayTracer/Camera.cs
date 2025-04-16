#pragma warning disable CA1416
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using RayTracer.RayMath;
using RayTracer.Shaders;
using Bitmap = System.Drawing.Bitmap;

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
            double focalLength = 0.028;
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

            var viewDir = new MyVector(
                -ray.Direction.X,
                -ray.Direction.Y,
                -ray.Direction.Z
            ).Normalize();

            var localPoint = intersection.Position.Copy();
            localPoint.Transform(ViewMatrix);

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

            Color reflectionColor = Color.Black;
            Color refractionColor = Color.Black;
            const double eps = 1e-6;

            if (kr > 0.0 && depth < MAX_DEPTH)
            {
                var R = ray.Direction
                          .Normalize()
                          .Reflect(intersection.Normal);
                var origin = new Point(
                    intersection.Position.X + R.X * eps,
                    intersection.Position.Y + R.Y * eps,
                    intersection.Position.Z + R.Z * eps
                );
                reflectionColor = TraceRay(new Ray(origin, R), world, depth + 1);
            }

            if (kt > 0.0 && depth < MAX_DEPTH)
            {
                var I = ray.Direction.Normalize();
                var N = intersection.Normal.Normalize();

                // Clamp index‐of‐refraction to at least 1.0:
                double materialIOR = Math.Max(1.0, intersection.Material.IndexOfRefraction);

                bool entering = I.Dot(N) < 0;
                double n1 = entering ? 1.0 : materialIOR;
                double n2 = entering ? materialIOR : 1.0;
                double eta = n1 / n2;

                double cosI = -I.Dot(N);
                double k = 1 - eta * eta * (1 - cosI * cosI);

                MyVector R;
                if (k < 0)
                {
                    // Total internal reflection
                    R = I.Reflect(N).Normalize();
                }
                else
                {
                    // Normal refraction
                    R = (eta * I + (eta * cosI - Math.Sqrt(k)) * N).Normalize();
                }

                var origin = new Point(
                    intersection.Position.X + R.X * 1e-6,
                    intersection.Position.Y + R.Y * 1e-6,
                    intersection.Position.Z + R.Z * 1e-6
                );
                refractionColor = TraceRay(new Ray(origin, R), world, depth + 1);
            }

            return (1 - kt) * localColor
                 + kr * reflectionColor
                 + kt * refractionColor;
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
            double posX = topLeftPixel.X + currentX * pixelWidth;
            double posY = topLeftPixel.Y - currentY * pixelHeight;
            var pixelCenter = new Point(posX, posY, _focalDistance);

            Color accumulatedColor = Color.Black;
            var localRand = new Random(currentX + currentY + Environment.TickCount);

            for (int sample = 0; sample < _samplesPerPixel; sample++)
            {
                var (diskX, diskY) = RandomInUnitDisk(localRand);
                diskX *= _apertureRadius;
                diskY *= _apertureRadius;

                var lensOrigin = new Point(
                    _cameraCoordsOrigin.X + diskX,
                    _cameraCoordsOrigin.Y + diskY,
                    _cameraCoordsOrigin.Z
                );

                var idealDir = pixelCenter
                    .Subtract(_cameraCoordsOrigin)
                    .Normalize();
                var focalPoint = new Point(
                    _cameraCoordsOrigin.X + idealDir.X * Math.Abs(_focalDistance),
                    _cameraCoordsOrigin.Y + idealDir.Y * Math.Abs(_focalDistance),
                    _cameraCoordsOrigin.Z + idealDir.Z * Math.Abs(_focalDistance)
                );

                var newDir = focalPoint
                    .Subtract(lensOrigin)
                    .Normalize();
                var ray = new Ray(lensOrigin, newDir);

                // <-- transform the primary ray here, once -->
                accumulatedColor += TraceRay(ray.Transform(CamToWorld), world, 0);
            }

            var finalColor = accumulatedColor * (1.0 / _samplesPerPixel);
            return (currentX, currentY, finalColor);
        }

        public Bitmap render(World world)
        {
            world.BuildKdTree();
            var pixelWidth = _filmPlaneWidth / _imageWidth;
            var pixelHeight = _filmPlaneHeight / _imageHeight;

            var topLeftPixel = new Point(
                -(_filmPlaneWidth / 2) + (pixelWidth / 2),
                 (_filmPlaneHeight / 2) - (pixelHeight / 2),
                 _focalDistance
            );

            if (_parallel)
            {
                var pixelTasks = new List<Task<(int, int, Color)>>();
                for (int y = 0; y < _imageHeight; y++)
                    for (int x = 0; x < _imageWidth; x++)
                        pixelTasks.Add(
                            Task.Run(() => spawnOnePixel(
                                topLeftPixel,
                                x, y,
                                pixelWidth, pixelHeight,
                                world))
                        );

                Task.WaitAll(pixelTasks.ToArray());
                var bmp = new Bitmap(_imageWidth, _imageHeight);
                foreach (var t in pixelTasks)
                {
                    var (x, y, col) = t.Result;
                    bmp.SetPixel(x, y, col.ToSystemColor());
                }
                return bmp;
            }
            else
            {
                var bmp = new Bitmap(_imageWidth, _imageHeight);
                for (int y = 0; y < _imageHeight; y++)
                    for (int x = 0; x < _imageWidth; x++)
                    {
                        var (ix, iy, col) = spawnOnePixel(
                            topLeftPixel,
                            x, y,
                            pixelWidth, pixelHeight,
                            world);
                        bmp.SetPixel(ix, iy, col.ToSystemColor());
                    }
                return bmp;
            }
        }
    }
}

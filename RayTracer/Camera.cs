#pragma warning disable CA1416
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void Supersample()
        {
            _apertureRadius = 0;
            _samplesPerPixel = 25;
            //_samplesPerPixel = 2;
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
                var N0 = intersection.Normal.Normalize();

                bool entering = I.Dot(N0) < 0;
                var N = entering ? N0 : N0 * -1.0;

                double materialIOR = Math.Max(1e-6, intersection.Material.IndexOfRefraction); 
                double n1 = entering ? 1.0 : materialIOR;
                double n2 = entering ? materialIOR : 1.0;
                double eta = n1 / n2;

                double cosI = -I.Dot(N);
                double k = 1 - eta * eta * (1 - cosI * cosI);

                MyVector T;
                if (k < 0)
                {
                    T = I.Reflect(N).Normalize();
                }
                else
                {
                    T = (eta * I + (eta * cosI - Math.Sqrt(k)) * N).Normalize();
                }

                var origin = new Point(
                    intersection.Position.X + T.X * 1e-6,
                    intersection.Position.Y + T.Y * 1e-6,
                    intersection.Position.Z + T.Z * 1e-6
                );
                refractionColor = TraceRay(new Ray(origin, T), world, depth + 1);

            }

            double kd = Math.Max(0.0, 1.0 - kr - kt);
            return kd * localColor
                 + kr * reflectionColor
                 + kt * refractionColor;
        }

        private Color SampleColor(Point pixelCenter, Random localRand, World world)
        {
            // --- 2) LENS‑DOF sampling (unchanged) ---
            (double diskX, double diskY) = RandomInUnitDisk(localRand);
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

            return TraceRay(ray.Transform(CamToWorld), world, 0);
        }

        private (int x, int y, Color color) SpawnOnePixel(
            Point topLeftPixel,
            int currentX,
            int currentY,
            double pixelWidth,
            double pixelHeight,
            World world
        )
        {
            Color accumulatedColor = Color.Black;

            var localRand = new Random(currentX + currentY * 800 + Environment.TickCount);

            for (int sample = 0; sample < _samplesPerPixel; sample++)
            {
                // --- 1) SUB‑PIXEL JITTER for anti‑aliasing ---
                double jitterX, jitterY;
                if (sample == 0)
                {
                    jitterX = 0;
                    jitterY = 0;
                } else
                {
                    jitterX = (localRand.NextDouble() - 0.5) * pixelWidth;
                    jitterY = (localRand.NextDouble() - 0.5) * pixelHeight;
                }

                double posX = topLeftPixel.X + currentX * pixelWidth + jitterX;
                double posY = topLeftPixel.Y - currentY * pixelHeight + jitterY;
                var pixelCenter = new Point(posX, posY, _focalDistance);

                accumulatedColor += SampleColor(pixelCenter, localRand, world);
            }

            var finalColor = accumulatedColor / _samplesPerPixel;
            return (currentX, currentY, finalColor);
        }

        public Bitmap Render(World world)
        {
            Stopwatch sw = Stopwatch.StartNew();
            world.BuildKdTree();
            sw.Stop();
            Console.WriteLine($"KD Tree Generation completed in: {sw.ElapsedMilliseconds} ms");
            var pixelWidth = _filmPlaneWidth / _imageWidth;
            var pixelHeight = _filmPlaneHeight / _imageHeight;

            var topLeftPixel = new Point(
                -(_filmPlaneWidth / 2) + (pixelWidth / 2),
                 (_filmPlaneHeight / 2) - (pixelHeight / 2),
                 _focalDistance
            );

            var bmp = new Bitmap(_imageWidth, _imageHeight);
            List<(int x, int y, Color color)> pixelResults = [];
            if (_parallel)
            {
                var pixelTasks = new List<Task<(int, int, Color)>>();
                for (int y = 0; y < _imageHeight; y++)
                    for (int x = 0; x < _imageWidth; x++)
                    {
                        int cx = x;  // capture copy
                        int cy = y;  // capture copy
                        pixelTasks.Add(Task.Run(() =>
                            SpawnOnePixel(
                              topLeftPixel,
                              cx, cy,
                              pixelWidth, pixelHeight,
                              world
                            )
                        ));
                    }
                Task.WaitAll(pixelTasks.ToArray());
                pixelResults = pixelTasks.Select(t => t.Result).ToList();
            }
            else
            {
                for (int y = 0; y < _imageHeight; y++)
                    for (int x = 0; x < _imageWidth; x++)
                    {
                        pixelResults.Add(SpawnOnePixel(
                            topLeftPixel,
                            x, y,
                            pixelWidth, pixelHeight,
                            world));
                    }
            }

            foreach (var (x, y, col) in pixelResults)
            {
                bmp.SetPixel(x, y, col.ToSystemColor());
            }

            return bmp;
        }
    }
}

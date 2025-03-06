using RayTracer.Objects;
using RayTracer.Tests;
using System.Diagnostics;
using System.IO;

namespace RayTracer
{
    public class Program
    {
        private static readonly string BaseLocation = @"C:\Users\Clinten\Documents\Courses\2245\GlobalIllum\RayTracer\";
        //private static readonly string OutputLocation = @"C:\Users\clint\source\repos\DevWalrus\CSCI711-RayTracer\";
        private static readonly string OutputLocation = BaseLocation + @"Output\";
        private static readonly string InputLocation = BaseLocation + @"Input\";
        private static bool _isParallel = false;

        static void Main(string[] args)
        {
            var argSet = new HashSet<string>(args);

            if (argSet.Contains("-p") || argSet.Contains("--parallel"))
                _isParallel = true;

            if (argSet.Contains("-t") || argSet.Contains("--test"))
                TestRunner.RunAllTests();
            else
                CreateRenderedImage();
        }

        static void CreateRenderedImage2()
        {
            var camera = new Camera(new Point(0, 0, 0), new Point(0, 0, -1), new MyVector(0, 1, 0), _isParallel);

            var world = new World();

            var lightSource = new LightSource(new Point(0, 10, -10), new Color(0, 0, 0));

            world.Add(lightSource);

            var red = new ColorShader(new Color(1, 0, 0));

            var transparentSphere = new Sphere(new Point(0, 0, -1), 0.5, red);

            world.Add(transparentSphere);

            var bitmap = camera.render(world);
            PPMWriter.WriteBitmapToPPM(OutputLocation + "SingleSphere.ppm", bitmap);
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + OutputLocation + "SingleSphere.ppm" + "\"";
            fileopener.Start();
        }

        static void CreateRenderedImage()
        {
            var camera = new Camera(new Point(0, 0.5, 1), new Point(0, 0, -1), new MyVector(0, 1, 0), _isParallel);
            camera.SetPinhole();

            var world = new World();

            var lightSource = new LightSource(new Point(0, 5, 2.5), new Color(1, 1, 1));

            world.Add(lightSource);

            var green = new ColorShader(new Color(0, 1, 0));
            var blue = new ColorShader(new Color(0, 0, 1));
            var greenPhong = new PhongShader(0.1, 0.6, 0.3, 16, green);
            var bluePhong = new PhongShader(0.1, 0.6, 0.3, 16, blue);
            var checkerboard = new CheckerboardShader(new Color(1, 0, 0), new Color(1, 1, 0), 0.1);
            var checkerboardPhong = new PhongShader(0.1, 0.6, 0.3, 16, checkerboard);

            var madelbrot = new MandelbrotShader(
                scaleX: 0.5,
                scaleY: 0.5,
                offsetX: -0.1,
                offsetY: -1,
                maxIterations: 100,
                rotationDegrees: 270
            );

            var madelbrotPhong = new PhongShader(0.1, 0.6, 0.3, 16, madelbrot);

            var transparentSphere = new Sphere(new Point(0, 0.4, -0.3), 0.2, greenPhong);
            var reflectiveSphere = new Sphere(new Point(0.2, 0.2, -0.5), 0.15, bluePhong);

            var rightSidePlane = new Triangle([new Point(1, 0, 1), new Point(-0.55, 0, 1), new Point(1, 0, -10)], new MyVector(0, 0, 1), madelbrotPhong);
            var leftSidePlane = new Triangle([new Point(-0.55, 0, 1), new Point(1, 0, -10), new Point(-0.55, 0, -10)], new MyVector(0, 0, 1), madelbrotPhong);
            
            world.Add(rightSidePlane);
            world.Add(leftSidePlane);
            world.Add(transparentSphere);
            world.Add(reflectiveSphere);

            Stopwatch sw = Stopwatch.StartNew();
            var bitmap = camera.render(world);
            sw.Stop();
            Console.WriteLine($"Render time: {sw.ElapsedMilliseconds} ms");
            PPMWriter.WriteBitmapToPPM(OutputLocation + "Scene.ppm", bitmap);
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + OutputLocation + "Scene.ppm" + "\"";
            fileopener.Start();
        }

        static void CreateRenderedDOFImage()
        {
            foreach (var app in new List<double>{1.4, 2, 2.8, 4, 5.6, 8, 11, 16, 22})
            {
                //Console.WriteLine(app);
                var camera = new Camera(new Point(0, 0.5, 1), new Point(0, 0, -1), new MyVector(0, 1, 0), false);
                camera.SetAperture(app);

                var world = new World();

                var lightSource = new LightSource(new Point(0, 5, 2.5), new Color(1, 1, 1));

                world.Add(lightSource);

                var green = new ColorShader(new Color(0, 1, 0));
                var blue = new ColorShader(new Color(0, 1, 0));
                var greenPhong = new PhongShader(0.1, 0.6, 0.3, 16, green);
                var bluePhong = new PhongShader(0.1, 0.6, 0.3, 16, blue);
                var checkerboard = new CheckerboardShader(new Color(1, 0, 0), new Color(1, 1, 0), 0.1);
                var checkerboardPhong = new PhongShader(0.1, 0.6, 0.3, 16, checkerboard);

                var transparentSphere = new Sphere(new Point(0, 0.4, -0.3), 0.2, greenPhong);
                var reflectiveSphere = new Sphere(new Point(0.2, 0.2, -0.5), 0.15, bluePhong);

                var rightSidePlane = new Triangle([new Point(1, 0, 1), new Point(-0.55, 0, 1), new Point(1, 0, -10)], new MyVector(0, 0, 1), checkerboardPhong);
                var leftSidePlane = new Triangle([new Point(-0.55, 0, 1), new Point(1, 0, -10), new Point(-0.55, 0, -10)], new MyVector(0, 0, 1), checkerboardPhong);

                world.Add(rightSidePlane);
                world.Add(leftSidePlane);
                world.Add(transparentSphere);
                world.Add(reflectiveSphere);

                var bitmap = camera.render(world);
                var f_name = $"Scene_DOF_{app.ToString().Replace(".", "_")}.ppm";
                PPMWriter.WriteBitmapToPPM(OutputLocation + f_name, bitmap);
                //using Process fileopener = new Process();

                //fileopener.StartInfo.FileName = "explorer";
                //fileopener.StartInfo.Arguments = "\"" + BaseOutputLocation + f_name + "\"";
                //fileopener.Start();
            }
        }
    }
}

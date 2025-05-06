using RayTracer.RayMath;
using RayTracer.Objects;
using RayTracer.Shaders;
using RayTracer.Tests;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra.Factorization;
using RayTracer.Utils;

namespace RayTracer
{
    public class Program
    {
        private static string BaseLocation = @"C:\";
        private static readonly string OutputFolder = @"Output\";
        private static readonly string InputFolder = @"Input\";
        private static string OutputLocation = BaseLocation + OutputFolder;
        private static string InputLocation = BaseLocation + InputFolder;
        private static bool _isParallel = false;

        static void Main(string[] args)
        {
            var argSet = new HashSet<string>(args);

            if (argSet.Contains("-m") || argSet.Contains("--multithreaded"))
                _isParallel = true;

            var pathArgIndex = Array.FindIndex(args, arg => arg == "-p" || arg == "--path");
            if (pathArgIndex != -1 && pathArgIndex + 1 < args.Length)
            {
                BaseLocation = args[pathArgIndex + 1];
                OutputLocation = Path.Combine(BaseLocation, OutputFolder);
                InputLocation = Path.Combine(BaseLocation, InputFolder);
            }

            if (argSet.Contains("-t") || argSet.Contains("--test"))
            {
                TestRunner.RunAllTests(InputLocation);
            }
            else if(argSet.Contains("-b") || argSet.Contains("--bunny"))
            {
                //CreateBunny();
                CreatePiano();
            }
            else
            {
                CreateRenderedImage();
            }
        }

        static void CreateRenderedImage()
        {
            var camera = new Camera(new Point(0, 0.5, 1), new Point(0, 0, -1), new MyVector(0, 1, 0), _isParallel);
            camera.SetPinhole();

            var world = new World(Color.SkyBlue);

            var lightSource = new LightSource(new Point(0, 5, 2.5), Color.White);

            world.Add(lightSource);

            var green = new ColorShader(Color.Green);
            var silver = new ColorShader(new Color(0.7529, 0.7529, 0.7529));
            var greenPhong = new PhongShader(0.1, 0.6, 0.3, 16, silver, indexOfRefraction: 0.95, transparency: 0.8);
            var silverPhong = new PhongShader(0.1, 0.6, 0.3, 16, silver, reflectivity: 0.75);
            //var floor = new NoisyCheckerboardShader(Color.Red, Color.Yellow, 0.1, 0.5);
            var floor = new CheckerboardShader(Color.Red, Color.Yellow, 0.1);
            //var floor = new BrickShader(Color.Red, Color.Yellow, 0.25, 0.5, 0.1);
            //var floor = new ImageShader(InputLocation + "joe.jpg", 0.5);
            //var floor = new MandelbrotShader(0.5, 0.5, -0.1, -1, 100, 270);
            var floorPhong = new PhongShader(0.1, 0.6, 0.3, 16, floor);

            var transparentSphere = new Sphere(new Point(0, 0.4, -0.3), 0.2, greenPhong);
            var reflectiveSphere = new Sphere(new Point(0.2, 0.2, -0.525), 0.15, silverPhong);

            var rightSidePlane = new Triangle(
                [new Point(1, 0, 1), new Point(-0.55, 0, 1), new Point(1, 0, -10)],
                [(0, 0), (1, 0), (0, 1)],
                new MyVector(0, 0, 1), 
                floorPhong);
            var leftSidePlane = new Triangle(
                [new Point(-0.55, 0, 1), new Point(1, 0, -10), new Point(-0.55, 0, -10)],
                [(1, 0), (0, 1), (1, 1)], 
                new MyVector(0, 0, 1), 
                floorPhong);
            
            world.Add(rightSidePlane);
            world.Add(leftSidePlane);
            world.Add(transparentSphere);
            world.Add(reflectiveSphere);

            Stopwatch sw = Stopwatch.StartNew();
            var bitmap = camera.Render(world);
            sw.Stop();
            Console.WriteLine($"Render time: {sw.ElapsedMilliseconds} ms");
            PPMWriter.WriteBitmapToPPM(Path.Combine(OutputLocation, "Scene.ppm"), bitmap);
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + Path.Combine(OutputLocation, "Scene.ppm") + "\"";
            fileopener.Start();
        }

        static void CreateBunny()
        {
            var camera = new Camera(new Point(-0.1, 0.25, 0.4), new Point(0, 0, -0.2), new MyVector(0, 1, 0), _isParallel);
            camera.SetPinhole();

            var world = new World(Color.SkyBlue);

            var lightSource = new LightSource(new Point(0, 5, 2.5), Color.White);

            world.Add(lightSource);

            var green = new ColorShader(Color.Green);
            var greenPhong = new PhongShader(0.1, 0.6, 0.3, 16, green);
            
            var bunny = new MeshObject(PlyParser.ParsePlyFile(Path.Combine(InputLocation, "bun_zipper.ply"), greenPhong), greenPhong);
            world.Add(bunny);

            Stopwatch sw = Stopwatch.StartNew();
            var bitmap = camera.Render(world);
            sw.Stop();
            Console.WriteLine($"Render time: {sw.ElapsedMilliseconds} ms");
            PPMWriter.WriteBitmapToPPM(Path.Combine(OutputLocation, "Bunny.ppm"), bitmap);
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + Path.Combine(OutputLocation, "Bunny.ppm") + "\"";
            fileopener.Start();
        }

        static void CreatePiano()
        {
            var world = new World(Color.SkyBlue);

            var lightSource = new LightSource(new Point(0, 5, 2.5), Color.White);

            world.Add(lightSource);

            var greenPhong = new PhongShader(0.1, 0.6, 0.3, 16, ColorShader.Green);
            var floor = new ImageShader(Path.Combine(InputLocation, "carpet.jpg"), 5);
            var floorPhong = new PhongShader(0.3, 0.6, 0.1, 16, floor);

            var piano = new MeshObject(ObjParser.ParseObjFile(Path.Combine(InputLocation, "Piano.obj"), greenPhong), greenPhong);
            world.Add(piano);

            var floorHeight = 0d;
            var rightSidePlane = new Triangle(
                [new Point(1, floorHeight, 1), new Point(-1, floorHeight, 1), new Point(1, floorHeight, -1)],
                [(1, 1), (0, 1), (1, 0)], 
                new MyVector(0, 0, 1), 
                floorPhong);
            var leftSidePlane = new Triangle(
                [new Point(-1, floorHeight, 1), new Point(1, floorHeight, -1), new Point(-1, floorHeight, -1)],
                [(0, 1), (1, 0), (0, 0)],
                new MyVector(0, 0, 1), 
                floorPhong);
            world.Add(rightSidePlane).Add(leftSidePlane);

            var camera = new Camera(new Point(0.6, 1.1, 0.2), new Point(-0.25, 0.25, 0.1), new MyVector(0, 1, 0), true);
            //camera.SetPinhole();
            camera.SetAperture(1.4);
            camera.Supersample();

            Stopwatch sw = Stopwatch.StartNew();
            var bitmap = camera.Render(world);
            sw.Stop();
            Console.WriteLine($"Render time: {sw.ElapsedMilliseconds} ms");
            PPMWriter.WriteBitmapToPPM(Path.Combine(OutputLocation, "Piano.ppm"), bitmap);
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + Path.Combine(OutputLocation, "Piano.ppm") + "\"";
            fileopener.Start();
        }

        static void CreateRenderedDOFImage()
        {
            foreach (var app in new List<double>{1.4, 2, 2.8, 4, 5.6, 8, 11, 16, 22})
            {
                //Console.WriteLine(app);
                var camera = new Camera(new Point(0, 0.5, 0.5), new Point(0, 0, -1), new MyVector(0, 1, 0), true);
                camera.SetPinhole();
                //camera.SetAperture(app);
                //camera.Supersample();


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

                var rightSidePlane = new Triangle(
                    [new Point(1, 0, 1), new Point(-0.55, 0, 1), new Point(1, 0, -10)],
                    [(1, 0), (0, 1), (1, 1)],
                    new MyVector(0, 0, 1), 
                    checkerboardPhong);
                var leftSidePlane = new Triangle(
                    [new Point(-0.55, 0, 1), new Point(1, 0, -10), new Point(-0.55, 0, -10)],
                    [(1, 0), (0, 1), (1, 1)],
                    new MyVector(0, 0, 1), 
                    checkerboardPhong);

                world.Add(rightSidePlane);
                world.Add(leftSidePlane);
                world.Add(transparentSphere);
                world.Add(reflectiveSphere);

                var bitmap = camera.Render(world);
                var fileName = $"Scene_DOF_{app.ToString().Replace(".", "_")}.ppm";
                PPMWriter.WriteBitmapToPPM(Path.Combine(OutputLocation, fileName), bitmap);
                //using Process fileopener = new Process();

                //fileopener.StartInfo.FileName = "explorer";
                //fileopener.StartInfo.Arguments = "\"" + BaseOutputLocation + f_name + "\"";
                //fileopener.Start();
            }
        }
    }
}

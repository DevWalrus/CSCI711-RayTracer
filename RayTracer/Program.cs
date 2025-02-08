using RayTracer.Objects;
using RayTracer.Tests;
using System.Diagnostics;
using System.IO;

namespace RayTracer
{
    public class Program
    {
        private static readonly string BaseOutputLocation = @"C:\Users\Clinten\Documents\Courses\2245\GlobalIllum\RayTracer\Output\";

        static void Main(string[] args)
        {
            var argSet = new HashSet<string>(args);

            if (argSet.Contains("-t") || argSet.Contains("--test"))
                TestRunner.RunAllTests();
            else
                CreateRenderedImage();
        }

        static void CreateRenderedImage()
        {
            var camera = new Camera(new Point(0, 0.5, 1), new Point(0, 0, -1), new MyVector(0, 1, 0));

            var world = new World();

            var red = new Color(1, 0, 0);
            var green = new Color(0, 1, 0);
            var blue = new Color(0, 0, 1);
            var orange = new Color(1, 0.65, 0);

            var transparentSphere = new Sphere(new Point(0, 0.4, -0.3), 0.2, red);
            var reflectiveSphere = new Sphere(new Point(0.2, 0.2, -0.5), 0.15, orange);

            var rightSidePlane = new Triangle([new Point(1, 0, 1), new Point(-0.55, 0, 1), new Point(1, 0, -10)], new MyVector(0, 0, 1), green);
            var leftSidePlane = new Triangle([new Point(-0.55, 0, 1), new Point(1, 0, -10), new Point(-0.55, 0, -10)], new MyVector(0, 0, 1), blue);

            world.Add(rightSidePlane);
            world.Add(leftSidePlane);
            world.Add(transparentSphere);
            world.Add(reflectiveSphere);

            var bitmap = camera.render(world);
            PPMWriter.WriteBitmapToPPM(BaseOutputLocation + "Scene.ppm", bitmap);
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + BaseOutputLocation + "Scene.ppm" + "\"";
            fileopener.Start();
        }
    }
}

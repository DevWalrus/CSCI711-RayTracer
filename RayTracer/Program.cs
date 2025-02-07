using RayTracer.Objects;
using RayTracer.Tests;

namespace RayTracer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var argSet = new HashSet<string>(args);

            if (argSet.Contains("-t") || argSet.Contains("--test"))
                TestRunner.RunAllTests();
            else
                CreateRenderedImage();
        }

        static void CreateRealRenderedImage()
        {
            var camera = new Camera(new Point(0, 1, 10), new Point(0, 0, -1), new MyVector(0, 1, 0));

            var world = new World();

            var red = new Color(1, 0, 0);
            var orange = new Color(1, 0.65, 0);
            var blue = new Color(0, 1, 0);
            var green = new Color(0, 0, 1);

            //var floor1 = new Triangle([new Point(1.65, 0, -4), new Point(2.15, 0, -4), new Point(1.65, 10, -4)], new MyVector(0, 0, 1), red);
            //var floor2 = new Triangle([new Point(2.15, 0, -4), new Point(2.15, 10, -4), new Point(1.65, 10, -4)], new MyVector(0, 0, 1), red);

            //world.Add(floor1);
            //world.Add(floor2);

            //var reflectiveSphere = new Sphere(new Point(0.9, 0.7, -6.9), 0.85, blue);

            //world.Add(reflectiveSphere);

            var transparentSphere = new Sphere(new Point(0.15, 1, -7.5), 1, green);

            Console.WriteLine(transparentSphere.Center);

            world.Add(transparentSphere);

            camera.render(world);

            Console.WriteLine(transparentSphere.Center);
        }

        static void CreateRenderedImage()
        {
            var camera = new Camera(new Point(0, 0, 10), new Point(0, 0, -1), new MyVector(0, 1, 0));

            var world = new World();

            var red = new Color(1, 0, 0);
            var orange = new Color(1, 0.65, 0);
            var blue = new Color(0, 1, 0);
            var green = new Color(0, 0, 1);

            //var floor1 = new Triangle([new Point(1.65, 0, -4), new Point(2.15, 0, -4), new Point(1.65, 10, -4)], new MyVector(0, 0, 1), red);
            //var floor2 = new Triangle([new Point(2.15, 0, -4), new Point(2.15, 10, -4), new Point(1.65, 10, -4)], new MyVector(0, 0, 1), red);

            //world.Add(floor1);
            //world.Add(floor2);

            //var reflectiveSphere = new Sphere(new Point(0.9, 0.7, -6.9), 0.85, blue);

            //world.Add(reflectiveSphere);

            var transparentSphere = new Sphere(new Point(0, 0, 0), 3, green);

            Console.WriteLine(transparentSphere.Center);

            world.Add(transparentSphere);

            camera.render(world);

            Console.WriteLine(transparentSphere.Center);
        }
    }
}

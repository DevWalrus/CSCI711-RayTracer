using MathNet.Numerics.LinearAlgebra.Double;
using RayTracer.Utils;
using RayTracer.Objects;
using System.Reflection;

namespace RayTracer.Tests
{
    public static class TestRunner
    {
        public static void RunAllTests()
        {
            var testMethods = typeof(TestRunner).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name.StartsWith("Test") && m.ReturnType == typeof(bool));

            foreach (var method in testMethods)
            {
                bool result = (bool)method.Invoke(null, null)!;
                Console.WriteLine($"{method.Name}: {(result ? "Passed" : "Failed")}");
            }
        }

        public static bool Test_Sphere_InFront_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, 0, 0), 2, new Color(1, 1, 1));
            
            // When
            var returnedColor = sphere.Intersect(ray);

            // Then
            return returnedColor != null;
        }

        public static bool Test_Sphere_LookingAway_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(0, 1, 0).Normalize());
            var sphere = new Sphere(new Point(5, 0, 0), 2, new Color(1, 1, 1));

            // When
            var returnedColor = sphere.Intersect(ray);

            // Then
            return returnedColor == null;
        }

        public static bool Test_Sphere_RayOnTop_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, -2, 0), 2, new Color(1, 1, 1));

            // When
            var returnedColor = sphere.Intersect(ray);

            // Then
            return returnedColor != null;
        }

        public static bool Test_Sphere_RayAbove_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, -3, 0), 2, new Color(1, 1, 1));

            // When
            var returnedColor = sphere.Intersect(ray);

            // Then
            return returnedColor == null;
        }

        public static bool Test_Tri_InFront_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, 1), new Point(5, 0, -1)], new MyVector(0, 0, 0), new Color(1, 1, 1));

            // When
            var returnedColor = triangle.Intersect(ray);

            // Then
            return returnedColor != null;
        }

        public static bool Test_Tri_LookingAway_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(0, 1, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, 1), new Point(5, 0, -1)], new MyVector(0, 0, 0), new Color(1, 1, 1));

            // When
            var returnedColor = triangle.Intersect(ray);

            // Then
            return returnedColor == null;
        }

        public static bool Test_Tri_RayOnTop_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, 1), new Point(5, 0, 0)], new MyVector(0, 0, 0), new Color(1, 1, 1));

            // When
            var returnedColor = triangle.Intersect(ray);

            // Then
            return returnedColor != null;
        }

        public static bool Test_Tri_RayBelow_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, 1), new Point(5, 0, 0.5)], new MyVector(0, 0, 0), new Color(1, 1, 1));

            // When
            var returnedColor = triangle.Intersect(ray);

            // Then
            return returnedColor == null;
        }

        public static bool Test_ViewMatrix_AtOrigin_LookingForward()
        {
            // Given & When
            var camera = new Camera(new Point(0, 0, 0), new Point(0, 0, -1), new MyVector(0, 1, 0));

            // Then
            return MatrixComparer.AreEqual(camera.ViewMatrix.ToArray(), new double[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            });
        }

        public static bool Test_ViewMatrix_AtOrigin_LookingStraightUp()
        {
            // Given & When
            var camera = new Camera(new Point(0, 0, 0), new Point(0, 1, 0), new MyVector(0, 0, 1));

            // Then
            return MatrixComparer.AreEqual(camera.ViewMatrix.ToArray(), new double[,]
            {
                { 1,  0, 0, 0 },
                { 0,  0, 1, 0 },
                { 0, -1, 0, 0 },
                { 0,  0, 0, 1 }
            });
        }

        public static bool Test_ViewMatrix_At_5_5_5_LookingAtOrigin()
        {
            // Given & When
            var camera = new Camera(new Point(5, 5, 5), new Point(0, 0, 0), new MyVector(0, 1, 0));

            // Then
            return MatrixComparer.AreEqual(camera.ViewMatrix.ToArray(), new double[,]
            {
                {   0.707,  0.000, -0.707,  0.000 },
                {  -0.408,  0.816, -0.408,  0.000 },
                {   0.577,  0.577,  0.577, -8.660 },
                {   0.000,  0.000,  0.000,  1.000 }
            }, 1e-3);
        }
    }
}

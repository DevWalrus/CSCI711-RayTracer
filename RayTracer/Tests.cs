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

        public static bool Test_Sphere_GivenInFront_WhenIntersecionIsCalculated_ThenThereShouldBeAnIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, 0, 0), 2, new Color(1, 1, 1));
            
            // When
            var returnedColor = sphere.Intersect(ray);

            // Then
            return returnedColor != null;
        }

        public static bool Test_Sphere_GivenLookingAway_WhenIntersecionIsCalculated_ThenThereShouldBeNoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(0, 1, 0).Normalize());
            var sphere = new Sphere(new Point(5, 0, 0), 2, new Color(1, 1, 1));

            // When
            var returnedColor = sphere.Intersect(ray);

            // Then
            return returnedColor == null;
        }

        public static bool Test_Sphere_GivenRayOnTop_WhenIntersecionIsCalculated_ThenThereShouldBeAnIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, -2, 0), 2, new Color(1, 1, 1));

            // When
            var returnedColor = sphere.Intersect(ray);

            // Then
            return returnedColor != null;
        }

        public static bool Test_Sphere_GivenRayAbove_WhenIntersecionIsCalculated_ThenThereShouldBeNoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, -3, 0), 2, new Color(1, 1, 1));

            // When
            var returnedColor = sphere.Intersect(ray);

            // Then
            return returnedColor == null;
        }

        public static bool Test_Tri_GivenInFront_WhenIntersecionIsCalculated_ThenThereShouldBeAnIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, 1), new Point(5, 0, -1)], new MyVector(0, 0, 0), new Color(1, 1, 1));

            // When
            var returnedColor = triangle.Intersect(ray);

            // Then
            return returnedColor != null;
        }

        public static bool Test_Tri_GivenLookingAway_WhenIntersecionIsCalculated_ThenThereShouldBeNoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(0, 1, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, 1), new Point(5, 0, -1)], new MyVector(0, 0, 0), new Color(1, 1, 1));

            // When
            var returnedColor = triangle.Intersect(ray);

            // Then
            return returnedColor == null;
        }

        public static bool Test_Tri_GivenRayOnTop_WhenIntersecionIsCalculated_ThenThereShouldBeNoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, 1), new Point(5, 0, 0)], new MyVector(0, 0, 0), new Color(1, 1, 1));

            // When
            var returnedColor = triangle.Intersect(ray);

            // Then
            return returnedColor != null;
        }

        public static bool Test_Tri_GivenRayBelow_WhenIntersecionIsCalculated_ThenThereShouldBeNoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, 1), new Point(5, 0, 0.5)], new MyVector(0, 0, 0), new Color(1, 1, 1));

            // When
            var returnedColor = triangle.Intersect(ray);

            // Then
            return returnedColor == null;
        }
    }
}

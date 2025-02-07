using MathNet.Numerics.LinearAlgebra.Double;
using RayTracer.Utils;
using RayTracer.Objects;
using System.Reflection;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualBasic;

namespace RayTracer.Tests
{
    public static class TestRunner
    {
        private static readonly string NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";
        private static readonly string RED = Console.IsOutputRedirected ? "" : "\x1b[91m";
        private static readonly string GREEN = Console.IsOutputRedirected ? "" : "\x1b[92m";

        private static readonly Color white = new Color(1, 1, 1);

        public static void RunAllTests()
        {
            var testMethods = typeof(TestRunner).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name.StartsWith("Test") && m.ReturnType == typeof(bool));

            foreach (var method in testMethods)
            {
                bool result = (bool)method.Invoke(null, null)!;
                Console.WriteLine($"{method.Name}: {(result ? $"{GREEN}Passed{NORMAL}" : $"{RED}Failed{NORMAL}")}");
            }
        }

        public static bool Test_Sphere_InFront_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, 0, 0), 2, white);
            
            // When
            var intersection = sphere.Intersect(ray);

            // Then
            return intersection?.Equals(new Interseciton(3, white)) ?? false;
        }

        public static bool Test_Sphere_InFront_Intersection_MinIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, 0, 0), 2, white);

            // When
            var intersection = sphere.Intersect(ray, 5);
            
            // Then
            return intersection?.Equals(new Interseciton(7, white)) ?? false;
        }

        public static bool Test_Sphere_LookingAway_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(0, 1, 0).Normalize());
            var sphere = new Sphere(new Point(5, 0, 0), 2, white);

            // When
            var intersection = sphere.Intersect(ray);

            // Then
            return intersection == null;
        }

        public static bool Test_Sphere_RayOnTop_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, -2, 0), 2, white);

            // When
            var intersection = sphere.Intersect(ray);

            // Then
            return intersection?.Equals(new Interseciton(5, white)) ?? false;
        }

        public static bool Test_Sphere_RayAbove_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, -3, 0), 2, white);

            // When
            var intersection = sphere.Intersect(ray);

            // Then
            return intersection == null;
        }

        public static bool Test_Tri_InFront_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, -1), new Point(5, 0, -1)], new MyVector(0, 0, 0), white);

            // When
            var intersection = triangle.Intersect(ray);

            // Then
            return intersection?.Equals(new Interseciton(5, white)) ?? false;
        }

        public static bool Test_Tri_InFront_Intersection_MinIntersecton()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, -1), new Point(5, 0, -1)], new MyVector(0, 0, 0), white);

            // When
            var intersection = triangle.Intersect(ray, 6);

            // Then
            return intersection == null;
        }

        public static bool Test_Tri_LookingAway_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(0, 1, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, -1), new Point(5, 0, -1)], new MyVector(0, 0, 0), white);

            // When
            var intersection = triangle.Intersect(ray);

            // Then
            return intersection == null;
        }

        public static bool Test_Tri_RayOnTop_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, -1), new Point(5, 0, -1)], new MyVector(0, 0, 0), white);

            // When
            var intersection = triangle.Intersect(ray);

            // Then
            return intersection?.Equals(new Interseciton(5, white)) ?? false;
        }

        public static bool Test_Tri_RayBelow_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, 1), new Point(5, 0, 0.5)], new MyVector(0, 0, 0), white);

            // When
            var intersection = triangle.Intersect(ray);

            // Then
            return intersection == null;
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

        public static bool Test_Point_Transformation_Scaling()
        {
            // Given
            var point = new Point(1, 2, 3);
            Matrix<double> transformMatrix = DenseMatrix.OfArray(new double[,]
            {
                { 2, 0, 0, 0 },
                { 0, 2, 0, 0 },
                { 0, 0, 2, 0 },
                { 0, 0, 0, 1 }
            });

            // When

            point.Transform(transformMatrix);

            // Then
            return point.CloseEquals(2, 4, 6);
        }

        public static bool Test_Point_Transformation_Translation()
        {
            // Given
            var point = new Point(1, 2, 3);
            Matrix<double> transformMatrix = DenseMatrix.OfArray(new double[,]
            {
                { 1, 0, 0, 2 },
                { 0, 1, 0, 2 },
                { 0, 0, 1, 2 },
                { 0, 0, 0, 1 }
            });

            // When

            point.Transform(transformMatrix);

            // Then
            return point.CloseEquals(3, 4, 5);
        }

        public static bool Test_Point_Transformation_Rotation_180_X()
        {
            // Given
            var point = new Point(1, 2, 3);
            Matrix<double> transformMatrix = DenseMatrix.OfArray(new double[,]
            {
                { 1,  0,  0, 0 },
                { 0, -1,  0, 0 },
                { 0,  0, -1, 0 },
                { 0,  0,  0, 1 }
            });

            // When

            point.Transform(transformMatrix);

            // Then
            return point.CloseEquals(1, -2, -3);
        }

        public static bool Test_Point_Transformation_Rotation_90_Y()
        {
            // Given
            var point = new Point(1, 0, 0);
            double angle = Math.PI / 2; // 90 degrees
            Matrix<double> transformMatrix = DenseMatrix.OfArray(new double[,]
            {
                { Math.Cos(angle),  0, Math.Sin(angle), 0 },
                { 0,                1,               0, 0 },
                { -Math.Sin(angle), 0, Math.Cos(angle), 0 },
                { 0,                0,               0, 1 }
            });

            // When

            point.Transform(transformMatrix);

            // Then
            return point.CloseEquals(0, 0, -1);
        }

        public static bool Test_Point_Transformation_Rotation_90_Z()
        {
            // Given
            var point = new Point(1, 0, 0);
            double angle = Math.PI / 2; // 90 degrees
            Matrix<double> transformMatrix = DenseMatrix.OfArray(new double[,]
            {
                { Math.Cos(angle), 0, -Math.Sin(angle), 0 },
                { Math.Sin(angle), 0,  Math.Cos(angle), 0 },
                { 0,               0,                1, 0 },
                { 0,               0,                0, 1 }

            // When
            });

            point.Transform(transformMatrix);

            // Then
            return point.CloseEquals(0, 1, 0);
        }


        public static bool Test_Point_Transformation_Combination()
        {
            // Given
            var point = new Point(1, 1, 1);

            // Scaling matrix (scale by 2)
            Matrix<double> scalingMatrix = DenseMatrix.OfArray(new double[,]
            {
                { 2, 0, 0, 0 },
                { 0, 2, 0, 0 },
                { 0, 0, 2, 0 },
                { 0, 0, 0, 1 }
            });

            // Rotation matrix (90 degrees around Z-axis)
            double angle = Math.PI / 2; // 90 degrees
            Matrix<double> rotationMatrix = DenseMatrix.OfArray(new double[,]
            {
                { Math.Cos(angle), -Math.Sin(angle), 0, 0 },
                { Math.Sin(angle),  Math.Cos(angle), 0, 0 },
                {               0,                0, 1, 0 },
                {               0,                0, 0, 1 }
            });

            // Translation matrix (shift by 2,2,2)
            Matrix<double> translationMatrix = DenseMatrix.OfArray(new double[,]
            {
                { 1, 0, 0, 2 },
                { 0, 1, 0, 2 },
                { 0, 0, 1, 2 },
                { 0, 0, 0, 1 }
            });

            Matrix<double> finalMatrix = translationMatrix * (rotationMatrix * scalingMatrix);

            // When: Apply scaling (2x), rotation (90° Z), and translation (2,2,2)
            point.Transform(finalMatrix);

            // Then: Expected (0, 4, 4)
            return point.CloseEquals(0, 4, 4);
        }

        public static bool Test_Point_MoveIntoCamSpace()
        {
            // Given
            var point = new Point(1, 2, 3);
            var camera = new Camera(new Point(0, 0, 5), new Point(0, 0, 0), new MyVector(0, 1, 0));

            // When
            point.Transform(camera.ViewMatrix);

            // Then
            return point.CloseEquals(1, 2, -2);
        }

        public static bool Test_Point_MoveIntoComplexCamSpace()
        {
            // Given
            var point = new Point(3, 4, 5);
            var camera = new Camera(new Point(2, 3, 10), new Point(0, 0, 0), new MyVector(0, 1, 0));

            // When
            point.Transform(camera.ViewMatrix);

            // Then
            return point.CloseEquals(1.96116135, 2.28768271, -4.23324391);
        }

        public static bool Test_Tri_MoveIntoCamSpace()
        {
            // Given
            var triangle = new Triangle([new Point(1, 2, 3), new Point(4, 5, 6), new Point(7, 8, 9)], new MyVector(0, 0, 0), new Color(0, 0, 0));
            var camera = new Camera(new Point(0, 0, 5), new Point(0, 0, 0), new MyVector(0, 1, 0));

            // When
            triangle.Transform(camera.ViewMatrix);

            // Then
            return triangle.A.CloseEquals(1, 2, -2) &&
                   triangle.B.CloseEquals(4, 5, 1) &&
                   triangle.C.CloseEquals(7, 8, 4);
        }

        public static bool Test_Tri_MoveIntoComplexCamSpace()
        {
            // Given
            var triangle = new Triangle([new Point(1, 2, 3), new Point(4, 5, 6), new Point(7, 8, 9)], new MyVector(0, 0, 0), new Color(0, 0, 0));
            var camera = new Camera(new Point(2, 3, 10), new Point(0, 0, 0), new MyVector(0, 1, 0));

            // When
            triangle.Transform(camera.ViewMatrix);

            // Then
            return triangle.A.CloseEquals(0.39223227, 1.03314703, -7.05540651) &&
                   triangle.B.CloseEquals(2.74562589, 2.91495055, -2.82216261) &&
                   triangle.C.CloseEquals(5.09901951, 4.79675406,  1.4110813);
        }

        public static bool Test_Sphere_MoveIntoCamSpace()
        {
            // Given
            var sphere = new Sphere(new Point(1, 2, 3), 2, new Color(0, 0, 0));
            var camera = new Camera(new Point(0, 0, 5), new Point(0, 0, 0), new MyVector(0, 1, 0));

            // When
            sphere.Transform(camera.ViewMatrix);

            // Then
            return sphere.Center.CloseEquals(1, 2, -2) &&
                sphere.Radius == 2;
        }

        public static bool Test_Sphere_MoveIntoComplexCamSpace()
        {
            // Given
            var sphere = new Sphere(new Point(1, 1, 1), 2, new Color(0, 0, 0));
            var camera = new Camera(new Point(2, 3, 10), new Point(0, 0, 0), new MyVector(0, 1, 0));

            // When
            sphere.Transform(camera.ViewMatrix);

            // Then
            return sphere.Center.CloseEquals(0.78446454, 0.62726784, -9.21906451) &&
                sphere.Radius == 2;
        }

        public static bool Test_Sphere_Scaled()
        {
            // Given
            var sphere = new Sphere(new Point(1, 1, 1), 2, new Color(0, 0, 0));
            Matrix<double> transformMatrix = DenseMatrix.OfArray(new double[,]
            {
                { 2, 0, 0, 0 },
                { 0, 2, 0, 0 },
                { 0, 0, 2, 0 },
                { 0, 0, 0, 1 }
            });

            // When
            sphere.Transform(transformMatrix);

            // Then
            return sphere.Center.CloseEquals(2, 2, 2) &&
                sphere.Radius == 4;
        }

        public static bool Test_Sphere_Translated()
        {
            // Given
            var sphere = new Sphere(new Point(1, 1, 1), 2, new Color(0, 0, 0));
            Matrix<double> transformMatrix = DenseMatrix.OfArray(new double[,]
            {
                { 1, 0, 0, 2 },
                { 0, 1, 0, 2 },
                { 0, 0, 1, 2 },
                { 0, 0, 0, 1 }
            });

            // When
            sphere.Transform(transformMatrix);

            // Then
            return sphere.Center.CloseEquals(3, 3, 3) &&
                sphere.Radius == 2;
        }

        public static bool Test_Sphere_Rotated_180_X()
        {
            // Given
            var sphere = new Sphere(new Point(1, 1, 1), 2, new Color(0, 0, 0));
            Matrix<double> transformMatrix = DenseMatrix.OfArray(new double[,]
            {
                { 1,  0,  0, 0 },
                { 0, -1,  0, 0 },
                { 0,  0, -1, 0 },
                { 0,  0,  0, 1 }
            });

            // When
            sphere.Transform(transformMatrix);

            // Then
            return sphere.Center.CloseEquals(1, -1, -1) &&
                sphere.Radius == 2;
        }
    }
}

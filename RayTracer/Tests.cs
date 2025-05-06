using MathNet.Numerics.LinearAlgebra.Double;
using RayTracer.Objects;
using System.Reflection;
using MathNet.Numerics.LinearAlgebra;
using RayTracer.Shaders;
using RayTracer.RayMath;
using RayTracer.Utils;

namespace RayTracer.Tests
{
    public static class TestRunner
    {
        private static readonly string NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";
        private static readonly string RED = Console.IsOutputRedirected ? "" : "\x1b[91m";
        private static readonly string GREEN = Console.IsOutputRedirected ? "" : "\x1b[92m";

        private static readonly Material WHITE_MAT = new ColorShader(Color.White);

        private static string InputLocation = @"C:\";

        private const double Tolerance = 1e-06;

        private static List<(int testNum, string? reason)> testResults = [];
        private static int currTestNum = 1;


        public static void RunAllTests(string inputLocation)
        {
            InputLocation = inputLocation;

            var testMethods = typeof(TestRunner).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name.StartsWith("Test"));

            foreach (var method in testMethods)
            {
                currTestNum = 1;
                method.Invoke(null, null);
                Console.Write($"{method.Name}:");
                foreach (var result in testResults)
                {
                    Console.Write($"\n\t#{result.testNum} - {(result.reason == null ? $"{GREEN}Passed{NORMAL}" : $"{RED}Failed: {result.reason}{NORMAL}")}");
                }
                Console.WriteLine();
                testResults.Clear();
            }
        }

        public static void Test_Sphere_InFront_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, 0, 0), 2, WHITE_MAT);

            // When
            var intersection = sphere.Intersect(ray);

            // Then
            Equals(intersection,
                new Intersection(3, new Point(3, 0, 0), new MyVector(-1, 0, 0), WHITE_MAT));
        }

        public static void Test_Sphere_InFront_Intersection_MinIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, 0, 0), 2, WHITE_MAT);

            // When
            var intersection = sphere.Intersect(ray, 5);

            // Then
            Equals(intersection,
                new Intersection(7, new Point(7, 0, 0), new MyVector(1, 0, 0), WHITE_MAT));
        }

        public static void Test_Sphere_LookingAway_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(0, 1, 0).Normalize());
            var sphere = new Sphere(new Point(5, 0, 0), 2, WHITE_MAT);

            // When
            var intersection = sphere.Intersect(ray);

            // Then
            IsNull(intersection);
        }

        public static void Test_Sphere_RayOnTop_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, -2, 0), 2, WHITE_MAT);

            // When
            var intersection = sphere.Intersect(ray);

            // Then
            Equals(intersection,
                new Intersection(5, new Point(5, 0, 0), new MyVector(0, 1, 0), WHITE_MAT));
        }

        public static void Test_Sphere_RayAbove_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var sphere = new Sphere(new Point(5, -3, 0), 2, WHITE_MAT);

            // When
            var intersection = sphere.Intersect(ray);

            // Then
            IsNull(intersection);
        }

        public static void Test_Tri_InFront_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle(
                [new Point(5, 1, 1), new Point(5, -1, -1), new Point(5, 0, -1)], 
                new MyVector(0, 1, 0), 
                WHITE_MAT);

            // When
            var intersection = triangle.Intersect(ray);
            // Then
            Equals(intersection, 
                new Intersection(5, new Point(5, 0, 0), new MyVector(0, 1, 0), WHITE_MAT));
        }

        public static void Test_Tri_InFront_Intersection_MinIntersecton()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, -1), new Point(5, 0, -1)], new MyVector(0, 1, 0), WHITE_MAT);

            // When
            var intersection = triangle.Intersect(ray, 6);

            // Then
            IsNull(intersection);
        }

        public static void Test_Tri_LookingAway_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(0, 1, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, -1), new Point(5, 0, -1)], new MyVector(0, 0, 0), WHITE_MAT);

            // When
            var intersection = triangle.Intersect(ray);

            // Then
            IsNull(intersection);
        }

        public static void Test_Tri_RayOnTop_Intersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, -1), new Point(5, 0, -1)], new MyVector(0, 1, 0), WHITE_MAT);

            // When
            var intersection = triangle.Intersect(ray);

            // Then
            Equals(intersection,
                new Intersection(5, new Point(5, 0, 0), new MyVector(0, 1, 0), WHITE_MAT));
        }

        public static void Test_Tri_RayBelow_NoIntersection()
        {
            // Given
            var ray = new Ray(new Point(0, 0, 0), new MyVector(1, 0, 0).Normalize());
            var triangle = new Triangle([new Point(5, 1, 1), new Point(5, -1, 1), new Point(5, 0, 0.5)], new MyVector(0, 0, 0), WHITE_MAT);

            // When
            var intersection = triangle.Intersect(ray);

            // Then
            IsNull(intersection);
        }

        public static void Test_ViewMatrix_AtOrigin_LookingForward()
        {
            // Given & When
            var camera = new Camera(new Point(0, 0, 0), new Point(0, 0, -1), new MyVector(0, 1, 0), false);

            // Then
            Equals(camera.ViewMatrix.ToArray(), new double[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            }, Tolerance);
        }

        public static void Test_ViewMatrix_AtOrigin_LookingStraightUp()
        {
            // Given & When
            var camera = new Camera(new Point(0, 0, 0), new Point(0, 1, 0), new MyVector(0, 0, 1), false);

            // Then
            Equals(camera.ViewMatrix.ToArray(), new double[,]
            {
                { 1,  0, 0, 0 },
                { 0,  0, 1, 0 },
                { 0, -1, 0, 0 },
                { 0,  0, 0, 1 }
            }, Tolerance);
        }

        public static void Test_ViewMatrix_At_5_5_5_LookingAtOrigin()
        {
            // Given & When
            var camera = new Camera(new Point(5, 5, 5), new Point(0, 0, 0), new MyVector(0, 1, 0), false);

            // Then
            Equals(camera.ViewMatrix.ToArray(), new double[,]
            {
                {   0.707,  0.000, -0.707,  0.000 },
                {  -0.408,  0.816, -0.408,  0.000 },
                {   0.577,  0.577,  0.577, -8.660 },
                {   0.000,  0.000,  0.000,  1.000 }
            }, 1e-3);
        }

        public static void Test_Point_Transformation_Scaling()
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
            CloseEquals(point, new Point(2, 4, 6));
        }

        public static void Test_Point_Transformation_Translation()
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
            CloseEquals(point, new Point(3, 4, 5));
        }

        public static void Test_Point_Transformation_Rotation_180_X()
        {
            // Given
            var point = new Point(1, 2, 3);
            Matrix<double> transformMatrix = TransformationMatrices.RotateX(Math.PI); // 180 degrees

            // When

            point.Transform(transformMatrix);

            // Then
            CloseEquals(point, new Point(1, -2, -3));
        }

        public static void Test_Point_Transformation_Rotation_90_Y()
        {
            // Given
            var point = new Point(1, 0, 0);
            double angle = Math.PI / 2; // 90 degrees
            Matrix<double> transformMatrix = TransformationMatrices.RotateY(angle);

            // When

            point.Transform(transformMatrix);

            // Then
            CloseEquals(point, new Point(0, 0, -1));
        }

        public static void Test_Point_Transformation_Rotation_90_Z()
        {
            // Given
            var point = new Point(1, 0, 0);
            double angle = Math.PI / 2; // 90 degrees
            Matrix<double> transformMatrix = TransformationMatrices.RotateZ(angle);

            // When
            point.Transform(transformMatrix);

            // Then
            CloseEquals(point, new Point(0, 1, 0));
        }


        public static void Test_Point_Transformation_Combination()
        {
            // Given
            var point = new Point(1, 1, 1);

            // Scaling matrix (scale by 2)
            Matrix<double> scalingMatrix = TransformationMatrices.LinearScale(2);

            // Rotation matrix (90 degrees around Z-axis)
            double angle = Math.PI / 2; // 90 degrees
            Matrix<double> rotationMatrix = TransformationMatrices.RotateZ(angle);

            // Translation matrix (shift by 2,2,2)
            Matrix<double> translationMatrix = TransformationMatrices.Translate(2, 2, 2);

            Matrix<double> finalMatrix = translationMatrix * (rotationMatrix * scalingMatrix);

            // When: Apply scaling (2x), rotation (90° Z), and translation (2,2,2)
            point.Transform(finalMatrix);

            // Then: Expected (0, 4, 4)
            CloseEquals(point, new Point(0, 4, 4));
        }

        public static void Test_Point_MoveIntoCamSpace()
        {
            // Given
            var point = new Point(1, 2, 3);
            var camera = new Camera(new Point(0, 0, 5), new Point(0, 0, 0), new MyVector(0, 1, 0), false);

            // When
            point.Transform(camera.ViewMatrix);

            // Then
            CloseEquals(point, new Point(1, 2, -2));
        }

        public static void Test_Point_MoveIntoComplexCamSpace()
        {
            // Given
            var point = new Point(3, 4, 5);
            var camera = new Camera(new Point(2, 3, 10), new Point(0, 0, 0), new MyVector(0, 1, 0), false);

            // When
            point.Transform(camera.ViewMatrix);

            // Then
            CloseEquals(point, new Point(1.96116135, 2.28768271, -4.23324391));
        }

        public static void Test_Tri_MoveIntoCamSpace()
        {
            // Given
            var triangle = new Triangle([new Point(1, 2, 3), new Point(4, 5, 6), new Point(7, 8, 9)], new MyVector(0, 0, 0), WHITE_MAT);
            var camera = new Camera(new Point(0, 0, 5), new Point(0, 0, 0), new MyVector(0, 1, 0), false);

            // When
            triangle.Transform(camera.ViewMatrix);

            // Then
            CloseEquals(triangle.A, new Point(1, 2, -2));
            CloseEquals(triangle.B, new Point(4, 5, 1));
            CloseEquals(triangle.C, new Point(7, 8, 4));
        }

        public static void Test_Tri_MoveIntoComplexCamSpace()
        {
            // Given
            var triangle = new Triangle([new Point(1, 2, 3), new Point(4, 5, 6), new Point(7, 8, 9)], new MyVector(0, 0, 0), WHITE_MAT);
            var camera = new Camera(new Point(2, 3, 10), new Point(0, 0, 0), new MyVector(0, 1, 0), false);

            // When
            triangle.Transform(camera.ViewMatrix);

            // Then
            CloseEquals(triangle.A, new Point(0.39223227, 1.03314703, -7.05540651));
            CloseEquals(triangle.B, new Point(2.74562589, 2.91495055, -2.82216261));
            CloseEquals(triangle.C, new Point(5.09901951, 4.79675406, 1.4110813));
        }

        public static void Test_Sphere_MoveIntoCamSpace()
        {
            // Given
            var sphere = new Sphere(new Point(1, 2, 3), 2, WHITE_MAT);
            var camera = new Camera(new Point(0, 0, 5), new Point(0, 0, 0), new MyVector(0, 1, 0), false);

            // When
            sphere.Transform(camera.ViewMatrix);

            // Then
            CloseEquals(sphere.Center, new Point(1, 2, -3));
            Equals(sphere.Radius, 2);
        }

        public static void Test_Sphere_MoveIntoComplexCamSpace()
        {
            // Given
            var sphere = new Sphere(new Point(1, 1, 1), 2, WHITE_MAT);
            var camera = new Camera(new Point(2, 3, 10), new Point(0, 0, 0), new MyVector(0, 1, 0), false);

            // When
            sphere.Transform(camera.ViewMatrix);

            // Then
            CloseEquals(sphere.Center, new Point(0.78446454, 0.62726784, -9.21906451));
            Equals(sphere.Radius, 2);
        }

        public static void Test_Sphere_Scaled()
        {
            // Given
            var sphere = new Sphere(new Point(1, 1, 1), 2, WHITE_MAT);
            Matrix<double> transformMatrix = TransformationMatrices.LinearScale(2);

            // When
            sphere.Transform(transformMatrix);

            // Then
            CloseEquals(sphere.Center, new Point(2, 2, 2));
            Equals(sphere.Radius, 4);
        }

        public static void Test_Sphere_Translated()
        {
            // Given
            var sphere = new Sphere(new Point(1, 1, 1), 2, WHITE_MAT);
            Matrix<double> transformMatrix = TransformationMatrices.Translate(2, 2, 2);

            // When
            sphere.Transform(transformMatrix);

            // Then
            CloseEquals(sphere.Center, new Point(3, 3, 3));
            Equals(sphere.Radius, 2);
        }

        public static void Test_Sphere_Rotated_180_X()
        {
            // Given
            var sphere = new Sphere(new Point(1, 1, 1), 2, WHITE_MAT);
            Matrix<double> transformMatrix = TransformationMatrices.RotateX(Math.PI); // 180 degrees

            // When
            sphere.Transform(transformMatrix);

            // Then
            CloseEquals(sphere.Center, new Point(1, -1, -1));
            Equals(sphere.Radius, 2);
        }

        public static void Test_Vector_Reflection_FromAbove()
        {
            // Given
            var vector = new MyVector(0, -1, 0);
            var normal = new MyVector(0, 1, 0);

            // When
            var result = vector.Reflect(normal);

            // Then
            CloseEquals(result, new MyVector(0, 1, 0));
        }

        public static void Test_Vector_Reflection_FromAngleAbove()
        {
            // Given
            var vector = new MyVector(1, -1, 0);
            var normal = new MyVector(0, 1, 0);

            // When
            var result = vector.Reflect(normal);

            // Then
            CloseEquals(result, new MyVector(0.707107, 0.707107, 0)); // ~(1/sqrt(2), 1/sqrt(2), 0)
        }

        public static void Test_Vector_Reflection_NonUnit()
        {
            // Given
            var vector = new MyVector(2, 0, 1);
            var normal = new MyVector(0, 3, 0);

            // When
            var result = vector.Reflect(normal);

            // Then
            CloseEquals(result, new MyVector(0.894427, 0, 0.447214)); // ~(2/sqrt(5), 1/sqrt(5), 0)
        }

        public static void Test_PLY_Triangle_Processing()
        {
            // Given
            var inputPly = Path.Combine(InputLocation, "triangle.ply");

            // When
            var plyTriangles = PlyParser.ParsePlyFile(inputPly, WHITE_MAT);

            foreach (var triangle in plyTriangles)
            {
                Console.WriteLine(triangle.ToString());
            }

            // Then
        }

        public static void Test_PLY_ComplexShape_Processing()
        {
            // Given
            var inputPly = Path.Combine(InputLocation, "tetrahedron.ply");
            var targetTri = new Triangle(
                [
                    new Point(0, 0, 0),
                    new Point(1, 0, 0),
                    new Point(1, 1, 0)
                ],
                new MyVector(0, 0, 0),
                WHITE_MAT);

            // When
            var plyTriangles = PlyParser.ParsePlyFile(inputPly, WHITE_MAT);

            foreach (var triangle in plyTriangles)
            {
                Console.WriteLine(triangle.ToString());
            }

            // Then
        }

        public static void Test_NewMeshObject_Creation()
        {
            // Given
            var inputPly = Path.Combine(InputLocation, "triangle.ply");

            // When
            var plyTriangles = PlyParser.ParsePlyFile(inputPly, WHITE_MAT);

            foreach (var triangle in plyTriangles)
            {
                Console.WriteLine(triangle.ToString());
            }

            // Then
        }

        public static void Test_Mesh_TranslateTransform()
        {
            // Given
            var triangle = new Triangle(
                [
                    new Point(1, 2, 3),
                    new Point(4, 5, 6),
                    new Point(7, 8, 9)
                ],
                new MyVector(0, 0, 0),
                WHITE_MAT);

            // Create a MeshObject using the new constructor.
            var mesh = new MeshObject([triangle], WHITE_MAT);

            // When
            var translation = TransformationMatrices.Translate(1, 0, 0);
            mesh.Transform(translation);

            // Then
            var triTransformed = mesh.Triangles[0];
            CloseEquals(triTransformed.A, new Point(2, 2, 3));
            CloseEquals(triTransformed.B, new Point(5, 5, 6));
            CloseEquals(triTransformed.C, new Point(8, 8, 9));
        }

        public static void Test_ObjParser_Tetrahedron()
        {
            // Given
            var inputObj = Path.Combine(InputLocation, "tetrahedron.obj");

            // When
            var tris = ObjParser.ParseObjFile(inputObj, WHITE_MAT);

            // Then: should have 4 faces
            Equals(4, tris.Count);

            // Face #1: v1,v2,v3 => (0,0,0),(1,0,0),(0,1,0)
            CloseEquals(tris[0].A, new Point(0, 0, 0));
            CloseEquals(tris[0].B, new Point(1, 0, 0));
            CloseEquals(tris[0].C, new Point(0, 1, 0));

            // Face #2: v1,v2,v4 => (0,0,0),(1,0,0),(0,0,1)
            CloseEquals(tris[1].A, new Point(0, 0, 0));
            CloseEquals(tris[1].B, new Point(1, 0, 0));
            CloseEquals(tris[1].C, new Point(0, 0, 1));

            // Face #3: v2,v3,v4 => (1,0,0),(0,1,0),(0,0,1)
            CloseEquals(tris[2].A, new Point(1, 0, 0));
            CloseEquals(tris[2].B, new Point(0, 1, 0));
            CloseEquals(tris[2].C, new Point(0, 0, 1));

            // Face #4: v3,v1,v4 => (0,1,0),(0,0,0),(0,0,1)
            CloseEquals(tris[3].A, new Point(0, 1, 0));
            CloseEquals(tris[3].B, new Point(0, 0, 0));
            CloseEquals(tris[3].C, new Point(0, 0, 1));
        }


        public static void Test_Mesh_MoveIntoCamSpace()
        {
            // Given
            // Create a single triangle with vertices (1,2,3), (4,5,6), and (7,8,9)
            var triangle = new Triangle(
                [
                    new Point(1, 2, 3),
                    new Point(4, 5, 6),
                    new Point(7, 8, 9)
                ],
                new MyVector(0, 0, 0),
                WHITE_MAT);
            var mesh = new MeshObject([triangle], WHITE_MAT);
            var camera = new Camera(
                new Point(0, 0, 5),
                new Point(0, 0, 0),
                new MyVector(0, 1, 0),
                false);

            // When
            mesh.Transform(camera.ViewMatrix);

            // Then
            var triTransformed = mesh.Triangles[0];
            CloseEquals(triTransformed.A, new Point(1, 2, -2));
            CloseEquals(triTransformed.B, new Point(4, 5, 1));
            CloseEquals(triTransformed.C, new Point(7, 8, 4));
        }

        public static void Test_Mesh_Intersection_Transform()
        {
            // Given
            var triangle = new Triangle(
                [
                    new Point(0, 0, 0),
                    new Point(1, 0, 0),
                    new Point(0, 1, 0)
                ],
                new MyVector(0, 0, 1),
                WHITE_MAT
            );
            var mesh = new MeshObject([triangle], WHITE_MAT);
            var ray = new Ray(new Point(0.25, 0.25, -1), new MyVector(0, 0, 1));

            Console.WriteLine($"Mesh:\n\tMin: {mesh.BBMin}\n\tCenter: {mesh.Center}\n\tMax: {mesh.BBMax}");

            // When
            var hitBefore = mesh.Intersect(ray);
            var translation = TransformationMatrices.Translate(2, 0, 0);
            mesh.Transform(translation);
            var hitAfter = mesh.Intersect(ray);

            // Then
            IsNotNull(hitBefore);
            IsNull(hitAfter);

            Console.WriteLine($"Mesh:\n\tMin: {mesh.BBMin}\n\tCenter: {mesh.Center}\n\tMax: {mesh.BBMax}");
        }

        class DummyRenderable : RenderableObject
        {
            public DummyRenderable(Point bbMin, Point bbMax, Material material) : base(material)
            {
                BBMin = bbMin;
                BBMax = bbMax;
                // Compute the center from bbMin and bbMax.
                Center = new Point((bbMin.X + bbMax.X) * 0.5,
                                   (bbMin.Y + bbMax.Y) * 0.5,
                                   (bbMin.Z + bbMax.Z) * 0.5);
            }

            public override Intersection? Intersect(Ray ray, double minIntersection = 0)
            {
                // For testing AABB computation, the intersection here is irrelevant.
                return null;
            }

            public override void Transform(Matrix<double> transformationMatrix)
            {
                // For testing, you don’t need to support transforming a dummy.
            }
        }

        public static void Test_AABBAggregationFromRenderables()
        {
            // Create a few dummy renderables with fixed bounding boxes.
            var renderables = new List<RenderableObject>
                {
                    new DummyRenderable(new Point(0, 0, 0), new Point(1, 1, 1), WHITE_MAT),
                    new DummyRenderable(new Point(-1, -1, -1), new Point(0, 0, 0), WHITE_MAT),
                    new DummyRenderable(new Point(0.5, 0.5, 0.5), new Point(2, 2, 2), WHITE_MAT)
                };

            // The overall AABB should enclose all objects.
            // Expected aggregated min: (-1, -1, -1) and max: (2, 2, 2).
            AABB box = new AABB(renderables);

            CloseEquals(box.Min, new Point(-1, -1, -1));
            CloseEquals(box.Max, new Point(2, 2, 2));
        }

        public static void Test_AABBIntersection_RayHits()
        {
            // Define a box from (0,0,0) to (1,1,1).
            Point min = new Point(0, 0, 0);
            Point max = new Point(1, 1, 1);
            AABB box = new AABB(min, max);

            // Create a ray that originates to the left of the box and points right.
            // It should hit the box. For example, the ray origin is (-1, 0.5, 0.5)
            // with a direction of (1, 0, 0).
            Point rayOrigin = new Point(-1, 0.5, 0.5);
            MyVector direction = new MyVector(1, 0, 0);
            Ray ray = new Ray(rayOrigin, direction);

            bool intersects = box.Intersect(ray, out double tMin, out double tMax) != null; // TODO: actually test intersection

            IsTrue(intersects);
            // Expect tMin to be 1 (exactly hitting the face at x=0) within some tolerance.
            CloseEquals(1, tMin);
        }

        public static void Test_AABBIntersection_RayMisses()
        {
            // Define a box from (0,0,0) to (1,1,1).
            Point min = new Point(0, 0, 0);
            Point max = new Point(1, 1, 1);
            AABB box = new AABB(min, max);

            // Create a ray that originates above the box and points upward.
            Point rayOrigin = new Point(0.5, 2, 0.5);
            MyVector direction = new MyVector(0, 1, 0);
            Ray ray = new Ray(rayOrigin, direction);

            bool intersects = box.Intersect(ray, out double tMin, out double tMax) != null; //TODO: actually test intersection

            IsFalse(intersects);
        }



        private static void IsNotNull<T>(T obj1)
        {
            if (obj1 != null)
            {
                testResults.Add((currTestNum++, null));
                return;
            }
            testResults.Add((currTestNum++, $"The value is null when it was not expected to be."));
        }

        private static void IsNull<T>(T obj1)
        {
            if (obj1 == null)
            {
                testResults.Add((currTestNum++, null));
                return;
            }
            testResults.Add((currTestNum++, $"The value is not as expected. Expected: null, Actual: {obj1.ToString()}"));
        }

        private static void IsFalse(bool value)
        {
            if (!value)
            {
                testResults.Add((currTestNum++, null));
                return;
            }
            testResults.Add((currTestNum++, $"The value is not as expected. Expected: false, Actual: {value.ToString()}"));
        }

        private static void IsTrue(bool value)
        {
            if (value)
            {
                testResults.Add((currTestNum++, null));
                return;
            }
            testResults.Add((currTestNum++, $"The value is not as expected. Expected: true, Actual: {value.ToString()}"));
        }

        private static void CloseEquals(double? obj1, double? obj2)
        {
            if (obj1 != null && obj2 != null && obj2.Value - obj1.Value < Tolerance)
            {
                testResults.Add((currTestNum++, null));
                return;
            }
            testResults.Add((currTestNum++, $"The value is not as expected. Expected: {obj1?.ToString() ?? "null"}, Actual: {obj2?.ToString() ?? "null"}"));
        }

        private static void CloseEquals(ICloseEquality obj1, ICloseEquality obj2)
        {
            if (obj1 != null && obj2 != null && obj1.CloseEquals(obj2))
            {
                testResults.Add((currTestNum++, null));
                return;
            }
            testResults.Add((currTestNum++, $"The value is not as expected. Expected: {obj1?.ToString() ?? "null"}, Actual: {obj2?.ToString() ?? "null"}"));
        }

        private static void Equals<T>(T obj1, T obj2)
        {
            if (obj1 != null && obj2 != null && obj1.Equals(obj2))
            {
                testResults.Add((currTestNum++, null));
                return;
            }
            testResults.Add((currTestNum++, $"The value is not as expected. Expected: {obj1?.ToString() ?? "null"}, Actual: {obj2?.ToString() ?? "null"}"));
        }

        private static void Equals(double[,] m1, double[,] m2, double epsilon)
        {
            // Check if either matrix is null and report immediately.
            if (m1 == null || m2 == null)
            {
                testResults.Add((currTestNum++,
                    $"Matrix comparison failed: one of the matrices is null. Expected: {(m1 == null ? "null" : "non-null")}, Actual: {(m2 == null ? "null" : "non-null")}"));
                return;
            }

            // Verify that the dimensions match.
            if (m1.GetLength(0) != m2.GetLength(0) || m1.GetLength(1) != m2.GetLength(1))
            {
                testResults.Add((currTestNum++,
                    $"Matrix dimensions differ: Expected: {m1.GetLength(0)}x{m1.GetLength(1)}, Actual: {m2.GetLength(0)}x{m2.GetLength(1)}"));
                return;
            }

            int rows = m1.GetLength(0);
            int cols = m1.GetLength(1);
            List<string> differences = new List<string>();

            // Loop through every cell in the matrices.
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double expectedVal = m1[i, j];
                    double actualVal = m2[i, j];
                    double diff = Math.Abs(expectedVal - actualVal);

                    // If the difference is greater than the allowed epsilon, record the difference.
                    if (diff > epsilon)
                    {
                        differences.Add($"[{i},{j}] Expected: {expectedVal}, Actual: {actualVal} (Diff: {diff})");
                    }
                }
            }

            // If any differences were found, report them; otherwise, mark the test as successful.
            if (differences.Count > 0)
            {
                testResults.Add((currTestNum++, "Differences found: " + string.Join("; ", differences)));
            }
            else
            {
                testResults.Add((currTestNum++, null));
            }
        }
    }
}

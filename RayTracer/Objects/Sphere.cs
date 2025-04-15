using MathNet.Numerics.LinearAlgebra;
using RayTracer.RayMath;
using RayTracer.Shaders;

namespace RayTracer.Objects
{
    public class Sphere : RenderableObject
    {
        private Point _center;
        private double _radius;

        public Point Center { get => _center; }
        public double Radius { get => _radius; }

        public Sphere (Point center, double radius, Material material) : base (material)
        {
            _center = center;
            _radius = radius;
        }

        /// <inheritdoc/>
        public override Intersection? Intersect(Ray ray, double minIntersection = 0)
        {
            var originToCenter = ray.Origin.Subtract(_center);

            var a = ray.Direction.SelfDot();
            var b = 2 * originToCenter.Dot(ray.Direction);
            var c = originToCenter.SelfDot() - RayMath.Pow(_radius, 2);

            var disc = RayMath.Pow(b, 2) - (4 * a * c);

            if (disc < 0) return null; // No intersections

            var sqrt_disc = RayMath.Sqrt(disc);
            var hit1 = (-b - sqrt_disc) / (2 * a);
            var hit2 = (-b + sqrt_disc) / (2 * a);

            if (RayMath.Abs(hit1 - hit2) > 1e-6)
            {
                var minHit = RayMath.Min(hit1, hit2);
                var maxHit = RayMath.Max(hit1, hit2);
                if (minHit >= minIntersection)
                {
                    var intersectionPoint = new Point(
                        ray.Origin.X + ray.Direction.X * minHit,
                        ray.Origin.Y + ray.Direction.Y * minHit,
                        ray.Origin.Z + ray.Direction.Z * minHit
                    );

                    return new Intersection(minHit, intersectionPoint, intersectionPoint.Subtract(_center), Material);
                }
                else if (maxHit >= minIntersection)
                {
                    var intersectionPoint = new Point(
                        ray.Origin.X + ray.Direction.X * maxHit,
                        ray.Origin.Y + ray.Direction.Y * maxHit,
                        ray.Origin.Z + ray.Direction.Z * maxHit
                    );

                    return new Intersection(maxHit, intersectionPoint, intersectionPoint.Subtract(_center), Material);
                }
            }
            else if (hit1 >= minIntersection) // The points are the same (hitting an edge of the sphere), only check one point
            {
                var intersectionPoint = new Point(
                    ray.Origin.X + ray.Direction.X * hit1,
                    ray.Origin.Y + ray.Direction.Y * hit1,
                    ray.Origin.Z + ray.Direction.Z * hit1
                );

                return new Intersection(hit1, intersectionPoint, intersectionPoint.Subtract(_center), Material);
            }

            return null;
        }

        public override void Transform(Matrix<double> m)
        {
            _center.Transform(m);

            var subMatrix = m.SubMatrix(0, 3, 0, 3);

            double scaleX = subMatrix.Column(0).L2Norm();
            double scaleY = subMatrix.Column(1).L2Norm();
            double scaleZ = subMatrix.Column(2).L2Norm();

            // Use the average for uniform scaling
            double uniformScale = (scaleX + scaleY + scaleZ) / 3.0;

            _radius *= uniformScale;
        }
    }
}

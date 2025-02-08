using MathNet.Numerics.LinearAlgebra;

namespace RayTracer.Objects
{
    public class Sphere : RenderableObject
    {
        private Point _center;
        private double _radius;

        public Point Center { get => _center; }
        public double Radius { get => _radius; }

        public Sphere (Point center, double radius, Color material) : base (material)
        {
            _center = center;
            _radius = radius;
        }

        /// <inheritdoc/>
        public override Interseciton? Intersect(Ray ray, double minIntersection = 0)
        {
            var originToCenter = ray.Origin.Subtract(_center);

            var a = ray.Direction.SelfDot();
            var b = 2 * originToCenter.Dot(ray.Direction);
            var c = originToCenter.SelfDot() - Math.Pow(_radius, 2);

            var disc = Math.Pow(b, 2) - (4 * a * c);

            if (disc < 0) return null; // No intersections

            var sqrt_disc = Math.Sqrt(disc);
            var hit1 = (-b - sqrt_disc) / (2 * a);
            var hit2 = (-b + sqrt_disc) / (2 * a);

            if (Math.Abs(hit1 - hit2) > 1e-6)
            {
                var minHit = Math.Min(hit1, hit2);
                var maxHit = Math.Max(hit1, hit2);
                if (minHit >= minIntersection)
                {
                    return new Interseciton(minHit, material);
                }
                else if (maxHit >= minIntersection)
                {
                    return new Interseciton(maxHit, material);
                }
            }
            else if (hit1 >= minIntersection) // The points are the same (hitting an edge of the sphere), only check one point
            {
                return new Interseciton(hit1, material);
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

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
        public override Color? Intersect(Ray ray)
        {
            var originToCenter = ray.origin.Subtract(_center);

            var a = ray.direction.SelfDot();
            var b = 2 * originToCenter.Dot(ray.direction);
            var c = originToCenter.SelfDot() - Math.Pow(_radius, 2);

            var disc = Math.Pow(b, 2) - (4 * a * c);

            if (disc < 0) return null; // No intersections

            return material;
        }

        public void Transform(Matrix<double> m)
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

namespace RayTracer.Objects
{
    public class Sphere : RenderableObject
    {
        private Point _center;
        private float _radius;

        public Sphere (Point center, float radius, Color material) : base (material)
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
    }
}

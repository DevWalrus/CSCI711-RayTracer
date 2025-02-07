namespace RayTracer.Objects
{
    public class Triangle : RenderableObject
    {
        private List<Point> _verticies;
        private MyVector _normal;

        public Triangle(List<Point> verticies, MyVector normal, Color material) : base(material)
        {
            _verticies = verticies;
            _normal = normal;
        }

        /// <inheritdoc/>
        public override Color? Intersect(Ray ray)
        {
            var e1 = _verticies[1].Subtract(_verticies[0]);
            var e2 = _verticies[2].Subtract(_verticies[0]);

            var p = ray.direction.Cross(e2);

            var denom = p.Dot(e1);

            if (denom == 0) return null; // ray is parallel to triangle

            var t = ray.origin.Subtract(_verticies[0]);
            var q = t.Cross(e1);

            var omega = q.Dot(e1) / denom;

            if (omega < 0) return null; // intersection point is behind ray origin

            var u = p.Dot(t) / denom;
            var v = q.Dot(ray.direction) / denom;

            if ((u < 0) || (v < 0) || (u + v) > 1) return null; // intersection point is outside of triangle

            return material;
        }
    }
}

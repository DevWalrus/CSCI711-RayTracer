using MathNet.Numerics.LinearAlgebra;
using RayTracer.Shaders;

namespace RayTracer.Objects
{
    public class Triangle : RenderableObject
    {
        private List<Point> _verticies;
        private MyVector _normal;

        public Point A { get => _verticies[0]; }
        public Point B { get => _verticies[1]; }
        public Point C { get => _verticies[2]; }

        public Triangle(List<Point> verticies, MyVector normal, Material material) : base(material)
        {
            _verticies = verticies;
            _normal = normal;
        }

        /// <inheritdoc/>
        public override Intersection? Intersect(Ray ray, double minIntersection = 0)
        {
            var e1 = _verticies[1].Subtract(_verticies[0]);
            var e2 = _verticies[2].Subtract(_verticies[0]);

            var p = ray.Direction.Cross(e2);

            var denom = p.Dot(e1);

            if (denom == 0) return null; // ray is parallel to triangle

            var t = ray.Origin.Subtract(_verticies[0]);
            var q = t.Cross(e1);

            var omega = q.Dot(e2) / denom;

            if (omega < 0) return null; // intersection point is behind ray origin

            var u = p.Dot(t) / denom;
            var v = q.Dot(ray.Direction) / denom;

            if ((u < 0) || (v < 0) || (u + v) > 1) return null; // intersection point is outside of triangle

            if (omega >= minIntersection)
            {
                var intersectionPoint = new Point(
                    ray.Origin.X + ray.Direction.X * omega,
                    ray.Origin.Y + ray.Direction.Y * omega,
                    ray.Origin.Z + ray.Direction.Z * omega
                );

                return new Intersection(omega, intersectionPoint, _normal, material);
            }

            return null;
        }

        public override void Transform(Matrix<double> transformationMatrix)
        {
            _verticies[0].Transform(transformationMatrix);
            _verticies[1].Transform(transformationMatrix);
            _verticies[2].Transform(transformationMatrix);
        }
    }
}

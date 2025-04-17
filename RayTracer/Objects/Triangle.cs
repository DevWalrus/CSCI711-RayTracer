using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using RayTracer.RayMath;
using RayTracer.Shaders;

namespace RayTracer.Objects
{
    public class Triangle : RenderableObject
    {
        private readonly List<Point> _vertices;
        private MyVector _normal;

        public Point A { get => _vertices[0]; }
        public Point B { get => _vertices[1]; }
        public Point C { get => _vertices[2]; }

        public Triangle(
            List<Point> verticies, 
            MyVector normal, 
            Material material
        ) : base(
            material,
            Point.Center(verticies),
            Point.Min(verticies),
            Point.Max(verticies)
        )
        {
            _vertices = verticies;
            _normal = normal;
        }

        /// <inheritdoc/>
        public override Intersection? Intersect(Ray ray, double minIntersection = 0)
        {
            var e1 = _vertices[1].Subtract(_vertices[0]);
            var e2 = _vertices[2].Subtract(_vertices[0]);

            var p = ray.Direction.Cross(e2);

            var denom = p.Dot(e1);

            if (denom == 0) return null; // ray is parallel to triangle

            var t = ray.Origin.Subtract(_vertices[0]);
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

                return new Intersection(omega, intersectionPoint, _normal, Material);
            }

            return null;
        }

        public override void Transform(Matrix<double> transformationMatrix)
        {
            _vertices[0].Transform(transformationMatrix);
            _vertices[1].Transform(transformationMatrix);
            _vertices[2].Transform(transformationMatrix);
            this.Center = Point.Center(_vertices);
            this.BBMin = Point.Min(_vertices);
            this.BBMax = Point.Max(_vertices);

            MyVector edge1 = new MyVector(_vertices[0], _vertices[1]);
            MyVector edge2 = new MyVector(_vertices[0], _vertices[2]);
            _normal = edge1.Cross(edge2).Normalize();
        }

        public override string ToString()
        {
            return $"Points:\n\tA: {A}\n\tB: {B}\n\tC: {C}\n\tNormal: {_normal}";
        }

        public Point this[int index]
        {
            get
            {
                return index switch
                {
                    0 => _vertices[0],
                    1 => _vertices[1],
                    2 => _vertices[2],
                    _ => throw new IndexOutOfRangeException("Index must be 0, 1, or 2")
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        _vertices[0] = value;
                        break;
                    case 1:
                        _vertices[1] = value;
                        break;
                    case 2:
                        _vertices[2] = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Index must be 0, 1, or 2");
                }
            }
        }
    }
}

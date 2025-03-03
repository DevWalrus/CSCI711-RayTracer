using MathNet.Numerics.LinearAlgebra;
using RayTracer.Objects;

namespace RayTracer
{
    public class World
    {
        private readonly List<RenderableObject> _objectList = [];
        private readonly List<LightSource> _lightList = [];

        public List<RenderableObject> Objects { get => _objectList; }
        public List<LightSource> Lights { get => _lightList; }

        public void Add(RenderableObject toAdd)
        {
            _objectList.Add(toAdd);
        }

        public void Add(LightSource toAdd)
        {
            _lightList.Add(toAdd);
        }

        public void TransformAllObjects(Matrix<double> m)
        {
            _objectList.ForEach(o => o.Transform(m));
        }

        public Intersection? Spawn(Ray ray)
        {
            Intersection? closestIntersection = null;

            foreach (RenderableObject obj in _objectList)
            {
                var intersection = obj.Intersect(ray);
                if (intersection != null && (closestIntersection == null || closestIntersection.Omega > intersection.Omega))
                {
                    closestIntersection = intersection;
                }
            }

            return closestIntersection;
        }
    }
}

using MathNet.Numerics.LinearAlgebra;
using RayTracer.Objects;

namespace RayTracer
{
    internal class World
    {
        public List<RenderableObject> _objectList = new List<RenderableObject>();

        public void Add(RenderableObject toAdd)
        {
            _objectList.Add(toAdd);
        }

        public void TransformAllObjects(Matrix<double> m)
        {
            _objectList.ForEach(o => o.Transform(m));
        }

        public Interseciton? Spawn(Ray ray)
        {
            Interseciton? closestIntersection = null;

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

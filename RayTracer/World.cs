using MathNet.Numerics.LinearAlgebra;
using RayTracer.Objects;
using System.Numerics;

namespace RayTracer
{
    internal class World
    {
        public List<RenderableObject> _objectList = new List<RenderableObject>();

        public void Add(RenderableObject toAdd)
        {
            _objectList.Add(toAdd);
        }

        public void Transform(RenderableObject toAdd)
        {

        }

        public void TransformAllObjects(Matrix<double> m)
        {
            _objectList.ForEach(o => o.Transform(m));
        }

        public void Spawn(Ray ray)
        {
            // todo: make rays return value, need closest intersection

            foreach (RenderableObject obj in _objectList)
            {
                var intersection = obj.Intersect(ray);
                if (intersection != null)
                {
                    Console.WriteLine(intersection);
                }
            }
        }
    }
}

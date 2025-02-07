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

        public void TransformAllObjects()
        {

        }

        public void Spawn(Ray ray)
        {

        }
    }
}

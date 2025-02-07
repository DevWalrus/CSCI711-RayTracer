using MathNet.Numerics.LinearAlgebra;
using RayTracer.Objects;

namespace RayTracer
{
    internal class World
    {
        public List<RenderableObject> _objectList = new List<RenderableObject>();

        private Point _focalPoint = new Point(0, 0, -1);

        private int _imageWidth = 800;
        private int _imageHeight = 800;

        private double _filmPlaneWidth = 1;
        private double _filmPlaneHeight = 1;

        public void Add(RenderableObject toAdd)
        {
            _objectList.Add(toAdd);
        }

        public void TransformAllObjects(Matrix<double> m)
        {
            _objectList.ForEach(o => o.Transform(m));
        }

        public void Spawn(Ray ray)
        {
            // todo: make rays return value, need closest intersection

            var topCorner = new MyVector()

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

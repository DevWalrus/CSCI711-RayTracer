using MathNet.Numerics.LinearAlgebra;
using RayTracer.Objects;

namespace RayTracer
{
    public class World
    {
        private readonly List<RenderableObject> _objectList = [];
        private readonly List<LightSource> _lightList = [];

        public KdTreeNode? KdTreeRoot;
        public List<RenderableObject> Objects { get => _objectList; }
        public List<LightSource> Lights { get => _lightList; }
        public Color BackgroundColor;

        public World(): this(Color.Black) { }

        public World(Color backgroundColor)
        {
            BackgroundColor = backgroundColor;
        }

        public World Add(RenderableObject toAdd)
        {
            _objectList.Add(toAdd);
            return this;
        }

        public World Add(LightSource toAdd)
        {
            _lightList.Add(toAdd);
            return this;
        }

        public World TransformAllObjects(Matrix<double> m)
        {
            _objectList.ForEach(o => o.Transform(m));
            return this;
        }

        public World BuildKdTree()
        {
            KdTreeRoot = new KdTreeNode(_objectList, 0);
            return this;
        }

        public Intersection? Spawn(Ray ray)
        {
            if (KdTreeRoot != null)
            {
                return KdTreeRoot.Traverse(ray, double.MaxValue);
            }
            else
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
}

using RayTracer.RayMath;
using RayTracer.Objects;

namespace RayTracer
{
    public class KdTreeNode
    {
        private static readonly int THRESHOLD = 2;

        public readonly AABB BoundingBox;
        public readonly KdTreeNode? Left;
        public readonly KdTreeNode? Right;
        public readonly List<RenderableObject>? Objects;

        public bool IsLeaf => Objects != null;

        public KdTreeNode(List<RenderableObject> objects) : this(objects, 0) { }

        public KdTreeNode(List<RenderableObject> objects, int depth)
        {
            var bbox = new AABB(objects);
            BoundingBox = bbox;

            // Base case: if objects count is small enough, create a leaf node.
            if (objects.Count <= THRESHOLD)
            {
                Objects = objects;
                return;
            }

            int axis = depth % 3;
            objects.Sort((a, b) => a.Center[axis].CompareTo(b.Center[axis]));

            int mid = objects.Count / 2;
            var leftObjects = objects.Take(mid).ToList();
            var rightObjects = objects.Skip(mid).ToList();

            Left = new KdTreeNode(leftObjects, depth + 1);
            Right = new KdTreeNode(rightObjects, depth + 1);
        }

        public Intersection? Traverse(Ray ray, double closestHit)
        {
            // If the ray misses the bounding box, skip this node.
            if (!BoundingBox.Intersect(ray, out double tMin, out double tMax) || tMin > closestHit)
                return null;

            // If the node is a leaf, check all objects.
            if (IsLeaf)
            {
                Intersection? bestIntersection = null;
                foreach (var obj in Objects!)
                {
                    var intersection = obj.Intersect(ray);
                    if (intersection != null && intersection.Omega < closestHit)
                    {
                        closestHit = intersection.Omega;
                        bestIntersection = intersection;
                    }
                }
                return bestIntersection;
            }

            // Recursively check children.
            Intersection? leftIntersection = Left!.Traverse(ray, closestHit);
            if (leftIntersection != null)
                closestHit = leftIntersection.Omega;

            Intersection? rightIntersection = Right!.Traverse(ray, closestHit);

            return (leftIntersection == null) ? rightIntersection :
                   (rightIntersection == null) ? leftIntersection :
                   (leftIntersection.Omega < rightIntersection.Omega ? leftIntersection : rightIntersection);
        }
    }
}

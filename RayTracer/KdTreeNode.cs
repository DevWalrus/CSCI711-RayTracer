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

        public Intersection? Traverse(Ray ray, double closestHit, bool lowPoly = false)
        {
            // If the ray misses the bounding box, skip this node.
            var bbInt = BoundingBox.Intersect(ray, out double tMin, out double tMax);
            if (bbInt == null || tMin > closestHit)
                return null;

            if (lowPoly) return bbInt;

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

            // For internal nodes, compute which child's bounding box is hit first.
            double leftTMin = double.PositiveInfinity;
            double rightTMin = double.PositiveInfinity;
            bool leftIntersects = Left != null && Left.BoundingBox.Intersect(ray, out leftTMin, out _) != null;
            bool rightIntersects = Right != null && Right.BoundingBox.Intersect(ray, out rightTMin, out _) != null;

            Intersection? firstIntersection = null;
            Intersection? secondIntersection = null;

            // Traverse the child with the lower tMin first.
            if (leftIntersects && (!rightIntersects || leftTMin < rightTMin))
            {
                firstIntersection = Left!.Traverse(ray, closestHit);
                if (firstIntersection != null)
                {
                    closestHit = firstIntersection.Omega;
                    // If the intersection in the left child is closer than 
                    // the entry point of the right child, no need to check right.
                    if (!rightIntersects || firstIntersection.Omega < rightTMin)
                        return firstIntersection;
                }
                secondIntersection = Right != null ? Right.Traverse(ray, closestHit) : null;
            }
            else if (rightIntersects)
            {
                firstIntersection = Right!.Traverse(ray, closestHit);
                if (firstIntersection != null)
                {
                    closestHit = firstIntersection.Omega;
                    // If the intersection in the right child is closer than 
                    // the entry point of the left child, return early.
                    if (!leftIntersects || firstIntersection.Omega < leftTMin)
                        return firstIntersection;
                }
                secondIntersection = Left != null ? Left.Traverse(ray, closestHit) : null;
            }

            // Choose the closer intersection of the two children.
            if (firstIntersection == null) return secondIntersection;
            if (secondIntersection == null) return firstIntersection;
            return firstIntersection.Omega < secondIntersection.Omega ? firstIntersection : secondIntersection;
        }
    }
}

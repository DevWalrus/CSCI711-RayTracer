using RayTracer.RayMath;
using RayTracer.Shaders;

namespace RayTracer.Objects
{
    public class AABB
    {
        public Point Min { get; private set; }
        public Point Max { get; private set; }

        /// <summary>
        /// Constructs an AABB that bounds all the objects in the list using their stored bounding box properties.
        /// </summary>
        /// <param name="objects">List of renderable objects to enclose.</param>
        public AABB(List<RenderableObject> objects)
        {
            if (objects == null || objects.Count == 0)
                throw new ArgumentException("Cannot compute AABB for an empty list of objects.");

            // Initialize overall bounds from the first object
            Min = Point.Min([.. objects.Select(o => o.BBMin)]);
            Max = Point.Max([.. objects.Select(o => o.BBMax)]);
        }

        /// <summary>
        /// Constructs an AABB directly from minimum and maximum points.
        /// </summary>
        public AABB(Point min, Point max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Tests whether a given ray intersects the bounding box using the slab method.
        /// </summary>
        public Intersection? Intersect(Ray ray, out double tMin, out double tMax)
        {
            tMin = double.MinValue;
            tMax = double.MaxValue;
            if (ray.Direction.X != 0)
            {
                double tx1 = (Min.X - ray.Origin.X) / ray.Direction.X;
                double tx2 = (Max.X - ray.Origin.X) / ray.Direction.X;
                double tMinX = Math.Min(tx1, tx2);
                double tMaxX = Math.Max(tx1, tx2);
                tMin = Math.Max(tMin, tMinX);
                tMax = Math.Min(tMax, tMaxX);
            }
            else if (ray.Origin.X < Min.X || ray.Origin.X > Max.X) return null;

            if (ray.Direction.Y != 0)
            {
                double ty1 = (Min.Y - ray.Origin.Y) / ray.Direction.Y;
                double ty2 = (Max.Y - ray.Origin.Y) / ray.Direction.Y;
                double tMinY = Math.Min(ty1, ty2);
                double tMaxY = Math.Max(ty1, ty2);
                tMin = Math.Max(tMin, tMinY);
                tMax = Math.Min(tMax, tMaxY);
            }
            else if (ray.Origin.Y < Min.Y || ray.Origin.Y > Max.Y) return null;

            if (ray.Direction.Z != 0)
            {
                double tz1 = (Min.Z - ray.Origin.Z) / ray.Direction.Z;
                double tz2 = (Max.Z - ray.Origin.Z) / ray.Direction.Z;
                double tMinZ = Math.Min(tz1, tz2);
                double tMaxZ = Math.Max(tz1, tz2);
                tMin = Math.Max(tMin, tMinZ);
                tMax = Math.Min(tMax, tMaxZ);
            }
            else if (ray.Origin.Z < Min.Z || ray.Origin.Z > Max.Z) return null;

            if (tMax < tMin || tMax < 0)
                return null;

            double tHit = tMin >= 0 ? tMin : tMax;
            Point hitPos = ray.At(tHit);
            MyVector normal = ComputeNormal(hitPos);

            return new Intersection(tHit, hitPos, normal, Material.Default);
        }

        private MyVector ComputeNormal(Point hit)
        {
            const double epsilon = 1e-5;
            if (Math.Abs(hit.X - Min.X) < epsilon) return new MyVector(-1, 0, 0);
            if (Math.Abs(hit.X - Max.X) < epsilon) return new MyVector(1, 0, 0);
            if (Math.Abs(hit.Y - Min.Y) < epsilon) return new MyVector(0, -1, 0);
            if (Math.Abs(hit.Y - Max.Y) < epsilon) return new MyVector(0, 1, 0);
            if (Math.Abs(hit.Z - Min.Z) < epsilon) return new MyVector(0, 0, -1);
            return new MyVector(0, 0, 1); // Max.Z
        }
    }
}

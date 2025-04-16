using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RayTracer.RayMath
{
    public class Point : ICloseEquality
    {
        public double X;
        public double Y;
        public double Z;

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Point Average(Point[] points)
        {
            var xSum = 0.0;
            var ySum = 0.0;
            var zSum = 0.0;

            foreach (var point in points)
            {
                xSum += point.X;
                ySum += point.Y;
                zSum += point.Z;
            }

            return new Point(
                xSum / points.Length,
                ySum / points.Length,
                zSum / points.Length
            );
        }

        public static Point Aggregate(List<Point> points, Func<double, double, double> aggregator)
        {
            if (points == null || points.Count == 0)
                throw new ArgumentException("Points array must contain at least one element.", nameof(points));

            double xRes = points[0].X;
            double yRes = points[0].Y;
            double zRes = points[0].Z;

            foreach (var point in points.Skip(1))
            {
                xRes = aggregator(xRes, point.X);
                yRes = aggregator(yRes, point.Y);
                zRes = aggregator(zRes, point.Z);
            }

            return new Point(xRes, yRes, zRes);
        }

        public static Point Min(List<Point> points)
        {
            return Aggregate(points, Math.Min);
        }

        public static Point Max(List<Point> points)
        {
            return Aggregate(points, Math.Max);
        }

        public static Point Center(List<Point> points)
        {
            var sumPoint = Aggregate(points, (a, b) => a + b * 0.5);
            var pointsLen = points.Count;

            return new Point(
                sumPoint.X / pointsLen,
                sumPoint.Y / pointsLen,
                sumPoint.Z / pointsLen
            );
        }

        public MyVector Subtract(Point otherP)
        {
            return new MyVector(X - otherP.X, Y - otherP.Y, Z - otherP.Z);
        }

        public Point Subtract(double value)
        {
            return new Point(X - value, Y - value, Z - value);
        }

        public Point Add(double value)
        {
            return new Point(X + value, Y + value, Z + value);
        }

        public double Distance(Point otherP)
        {
            return Math.Sqrt(Math.Pow(otherP.X - X, 2) + Math.Pow(otherP.Y - Y, 2) + Math.Pow(otherP.Z - Z, 2));
        }

        public void Transform(Matrix<double> m)
        {
            var homogeneousPoint = DenseVector.OfArray([X, Y, Z, 1]);

            var transformed = m.TransformPoint(this);

            X = transformed.X;
            Y = transformed.Y;
            Z = transformed.Z;
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }

        private bool CloseEquals(Point otherP, double tolerance)
        {
            return Math.Abs(X - otherP.X) < tolerance && Math.Abs(Y - otherP.Y) < tolerance && Math.Abs(Z - otherP.Z) < tolerance;
        }

        private bool CloseEquals(double x, double y, double z, double tolerance)
        {
            return CloseEquals(new Point(x, y, z), tolerance);
        }

        public Point Copy()
        {
            return new Point(X, Y, Z);
        }

        public bool CloseEquals(object? obj, double tolerance = ICloseEquality.Tolerance)
        {
            if (obj is Point otherPoint)
            {
                return CloseEquals(otherPoint, tolerance);
            }
            else if (obj is double[] arr && arr.Length == 3)
            {
                return CloseEquals(arr[0], arr[1], arr[2], tolerance);
            }
            else
            {
                return false;
            }
        }

        public static Point operator *(Point p1, double c)
        {
            return new Point(p1.X * c, p1.Y * c, p1.Z * c);
        }

        public static Point operator *(double c, Point p1)
        {
            return p1 * c;
        }

        public double this[int index]
        {
            get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    _ => throw new IndexOutOfRangeException("Index must be 0, 1, or 2")
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Index must be 0, 1, or 2");
                }
            }
        }
    }
}

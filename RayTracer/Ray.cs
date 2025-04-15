using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Spatial.Euclidean;

namespace RayTracer
{
    public class Color
    {
        public static Color White = new Color(1, 1, 1);
        public static Color Black = new Color(0, 0, 0);
        public static Color Red = new Color(1, 0, 0);
        public static Color Green = new Color(0, 1, 0);
        public static Color Blue = new Color(0, 0, 1);
        public static Color Yellow = new Color(1, 1, 0);
        public static Color Cyan = new Color(0, 1, 1);
        public static Color Magenta = new Color(1, 0, 1);
        public static Color Gray = new Color(0.5, 0.5, 0.5);
        public static Color DarkGray = new Color(0.25, 0.25, 0.25);
        public static Color LightGray = new Color(0.75, 0.75, 0.75);

        public double R;
        public double G;
        public double B;

        public Color(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        public override string ToString()
        {
            return $"R: {R}, G: {G}, B: {B}";
        }

        public override bool Equals(object? obj) => (obj is Color other) && Equals(other);

        public bool Equals(Color other)
        {
            return Math.Abs(R - other.R) < 1e-6 &&
                Math.Abs(G - other.G) < 1e-6 &&
                Math.Abs(B - other.B) < 1e-6;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static Color operator *(Color m1, double c)
        {
            return new Color(m1.R * c, m1.G * c, m1.B * c);
        }

        public static Color operator *(double c, Color m1)
        {
            return m1 * c;
        }

        public static Color operator +(Color m1, Color m2)
        {
            return new Color(m1.R + m2.R, m1.G + m2.G, m1.B + m2.B);
        }

        public System.Drawing.Color ToSystemColor()
        {
            var r = Math.Min((int)(R * 255), 255);
            var g = Math.Min((int)(G * 255), 255);
            var b = Math.Min((int)(B * 255), 255);
            var sysC = System.Drawing.Color.FromArgb(255, r, g, b);
            return sysC;
        }

    }

    public class Point : ICloseEquality
    {
        public double X;
        public double Y;
        public double Z;

        public Point(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
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
            var sumPoint = Aggregate(points, (a, b) => a + b);
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

    public class MyVector : ICloseEquality
    {

        private static readonly VectorBuilder<double> V = Vector<double>.Build;

        public Vector3D vector;
        public double X { get => vector.X; }
        public double Y { get => vector.Y; }
        public double Z { get => vector.Z; }

        public static MyVector Zero = new MyVector(0, 0, 0);

        public MyVector(Point p1, Point p2) : this(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z) { }

        public MyVector(double x, double y, double z)
        {
            vector = Vector3D.OfVector(V.Dense([x, y, z]));
        }

        private MyVector(Vector3D vector)
        {
            this.vector = vector;
        }

        public MyVector Add(MyVector otherVector)
        {
            return new MyVector(vector + otherVector.vector);
        }

        public MyVector Subtract(MyVector otherVector)
        {
            return new MyVector(vector - otherVector.vector);
        }

        public MyVector Cross(MyVector otherVector)
        {
            return new MyVector(vector.CrossProduct(otherVector.vector));
        }

        public double Dot(MyVector otherVector)
        {
            return vector.DotProduct(otherVector.vector);
        }

        public double SelfDot()
        {
            return vector.DotProduct(vector);
        }

        public double Length()
        {
            return vector.Length;
        }

        public MyVector Transform(Matrix<double> m)
        {
            return new MyVector(vector.TransformBy(m));
        }

        public MyVector Normalize()
        {
            if (CloseEquals(Zero)) return this;
            vector = vector.Normalize().ToVector3D();
            return this;
        }

        public MyVector Reflect(MyVector normal)
        {
            var dirNormalized = this.Normalize();
            normal = normal.Normalize();

            var num = dirNormalized.Dot(normal);
            var denom = normal.SelfDot();

            var multipicant = 2 * (num / denom);

            var subtract = new MyVector(normal.X * multipicant, normal.Y * multipicant, normal.Z * multipicant);
            return dirNormalized.Subtract(subtract).Normalize();
        }

        public MyVector? Refract(MyVector normal, double ior)
        {
            double cosi = Math.Max(-1.0, Math.Min(1.0, Dot(normal)));
            double etai = 1.0;
            double etat = ior;
            MyVector n = normal;

            if (cosi < 0)
            {
                cosi = -cosi;
            }
            else
            {
                double temp = etai;
                etai = etat;
                etat = temp;
                n = new MyVector(-normal.X, -normal.Y, -normal.Z);
            }

            double eta = etai / etat;
            double k = 1 - eta * eta * (1 - cosi * cosi);

            if (k < 0)
            {
                return null;
            }
            else
            {
                MyVector refractedPart = this * eta;
                MyVector normalPart = n * (eta * cosi - Math.Sqrt(k));
                return refractedPart.Add(normalPart).Normalize();
            }
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }

        public static MyVector operator *(MyVector m1, double c)
        {
            return new MyVector(m1.X * c, m1.Y * c, m1.Z * c);
        }

        public static MyVector operator *(double c, MyVector m1)
        {
            return m1 * c;
        }

        private bool CloseEquals(MyVector otherV, double tolerance)
        {
            return Math.Abs(X - otherV.X) < tolerance && Math.Abs(Y - otherV.Y) < tolerance && Math.Abs(Z - otherV.Z) < tolerance;
        }

        public bool CloseEquals(object? obj, double tolerance = ICloseEquality.Tolerance)
        {
            if (obj is MyVector otherVector)
            {
                return CloseEquals(otherVector, tolerance);
            }
            else
            {
                return false;
            }
        }
    }

    public class Ray
    {
        public Point Origin;
        public MyVector Direction;

        public Ray(Point origin, MyVector direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public override string ToString()
        {
            return $"Origin: {Origin}, Direction: {Direction}";
        }
    }
}

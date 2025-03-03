using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Spatial.Euclidean;

namespace RayTracer
{
    public class Color
    {
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
            var sysC = System.Drawing.Color.FromArgb(r, g, b);
            return sysC;
        }

    }

    public class Point
    {
        private const double Tolerance = 1e-6; // Adjust precision as needed

        public double X;
        public double Y;
        public double Z;
        
        public Point(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public MyVector Subtract(Point otherP)
        {
            return new MyVector(X - otherP.X, Y - otherP.Y, Z - otherP.Z);
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

        public bool CloseEquals(Point otherP, double tolerance = Tolerance)
        {
            return Math.Abs(X - otherP.X) < tolerance && Math.Abs(Y - otherP.Y) < tolerance && Math.Abs(Z - otherP.Z) < tolerance;
        }

        public bool CloseEquals(double x, double y, double z, double tolerance = Tolerance)
        {
            return this.CloseEquals(new Point(x, y, z), tolerance);
        }

        public Point Copy()
        {
            return new Point(X, Y, Z);
        }
    }

    public class MyVector
    {
        private const double Tolerance = 1e-6; // Adjust precision as needed

        private static VectorBuilder<double> V = Vector<double>.Build;
        
        public Vector3D vector;
        public double X { get => vector.X; }
        public double Y { get => vector.Y; }
        public double Z { get => vector.Z; }

        public MyVector(double x, double y, double z) 
        {
            this.vector = Vector3D.OfVector(V.Dense([x, y, z]));
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

        public bool CloseEquals(MyVector otherV, double tolerance = Tolerance)
        {
            return Math.Abs(X - otherV.X) < tolerance && Math.Abs(Y - otherV.Y) < tolerance && Math.Abs(Z - otherV.Z) < tolerance;
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

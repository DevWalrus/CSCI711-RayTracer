using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;

namespace RayTracer.RayMath
{
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

        public static MyVector operator +(MyVector m1, MyVector m2)
        {
            return m1.Add(m2);
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
            var dirNormalized = Normalize();
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
}

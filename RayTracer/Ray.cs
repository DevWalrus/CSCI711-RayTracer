using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using System;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    public class Color
    {
        private double _r;
        private double _g;
        private double _b;

        public Color(double r, double g, double b)
        {
            _r = r;
            _g = g;
            _b = b;
        }
    }

    public class Point
    {
        public double x;
        public double y;
        public double z;

        public Point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public MyVector Subtract(Point otherP)
        {
            return new MyVector(x - otherP.x, y - otherP.y, z - otherP.z);
        }

        public double Distance(Point otherP)
        {
            return Math.Sqrt(Math.Pow(otherP.x - x, 2) + Math.Pow(otherP.y - y, 2) + Math.Pow(otherP.z - z, 2));
        }
    }

    public class MyVector
    {

        private static VectorBuilder<double> V = Vector<double>.Build;
        
        public Vector3D vector;

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

        public double X()
        {
            return vector.X;
        }

        public double Y()
        {
            return vector.Y;
        }

        public double Z()
        {
            return vector.Z;
        }
    }

    public class Ray
    {
        public Point origin;
        public MyVector direction;

        public Ray(Point origin, MyVector direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }
}

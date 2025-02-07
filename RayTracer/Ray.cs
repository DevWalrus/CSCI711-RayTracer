using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using System;

namespace RayTracer
{
    class Color
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

    class Point
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
            return new MyVector(otherP.x - x, otherP.y - y, otherP.z - z);
        }

        public double Distance(Point otherP)
        {
            return Math.Sqrt(Math.Pow(otherP.x - x, 2) + Math.Pow(otherP.y - y, 2) + Math.Pow(otherP.z - z, 2));
        }
    }

    class MyVector
    {
        private static VectorBuilder<double> V = Vector<double>.Build;
        
        public UnitVector3D vector;

        public MyVector(double x, double y, double z) : this(Vector3D.OfVector(V.Dense([x, y, z])).Normalize()) { }

        private MyVector(Vector3D vector) : this(vector.Normalize()) { }

        private MyVector(UnitVector3D vector)
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
    }



    internal class Ray
    {
        public Point origin;
        public MyVector direction;


    }
}

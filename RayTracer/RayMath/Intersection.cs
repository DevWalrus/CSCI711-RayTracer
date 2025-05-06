using System.Numerics;
using RayTracer.Shaders;

namespace RayTracer.RayMath
{
    public class Intersection
    {
        public double Omega { get; }
        public Point Position { get; }
        public MyVector Normal { get; }
        public Material Material { get; }

        public readonly (float U, float V) UV;

        public Intersection(double omega, Point position, MyVector normal, Material material): 
            this(omega, position, normal, material, (0, 0)) { }

        public Intersection(double omega, Point position, MyVector normal, Material material, (float U, float V) uv)
        {
            Omega = omega;
            Position = position;
            Normal = normal.Normalize();
            Material = material;
            UV = uv;
        }

        public override string ToString()
        {
            return $"Omega: {Omega}, Position: {Position}, Normal: {Normal}, Material: {Material}";
        }

        public override bool Equals(object? obj) => obj is Intersection other && Equals(other);

        public bool Equals(Intersection other)
        {
            return Math.Abs(Omega - other.Omega) < 1e-6 &&
                Position.CloseEquals(other.Position) &&
                Normal.CloseEquals(other.Normal) &&
                Material.Equals(other.Material);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}

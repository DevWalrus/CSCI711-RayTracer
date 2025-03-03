using System.Numerics;

namespace RayTracer
{
    public class Intersection
    {
        public double Omega { get; }
        public Point Position { get; }
        public MyVector Normal { get; }
        public Material Material { get; }

        public Intersection(double omega, Point position, MyVector normal, Material material)
        {
            Omega = omega;
            Position = position;
            Normal = normal.Normalize();
            Material = material;
        }

        public override string ToString()
        {
            return $"Omega: {Omega}, Position: {Position}, Normal: {Normal}, Material: {Material}";
        }

        public override bool Equals(object? obj) => (obj is Intersection other) && Equals(other);

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

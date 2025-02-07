namespace RayTracer
{
    public class Interseciton
    {
        public double Omega;
        public Color Material;

        public Interseciton(double omega, Color material)
        {
            Omega = omega;
            Material = material;
        }

        public override string ToString()
        {
            return $"Omega: {Omega}, Material: {Material}";
        }

        public override bool Equals(object? obj) => (obj is Interseciton other) && Equals(other);

        public bool Equals(Interseciton other)
        {
            return Math.Abs(Omega - other.Omega) < 1e-6 && Material.Equals(other.Material);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}

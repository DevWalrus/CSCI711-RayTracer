namespace RayTracer.RayMath
{
    public interface ICloseEquality
    {
        protected const double Tolerance = 1e-6; // Adjust precision as needed

        bool CloseEquals(object? obj, double tolerance = Tolerance);
    }
}

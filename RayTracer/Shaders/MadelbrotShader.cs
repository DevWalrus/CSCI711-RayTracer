using RayTracer.RayMath;

namespace RayTracer.Shaders
{
    /// <summary>
    /// A shader that renders the Mandelbrot set by mapping an object's local (x,z) coordinates
    /// onto the complex plane and iterating the Mandelbrot formula.
    /// </summary>
    public class MandelbrotShader : Material
    {
        private readonly double scaleX;
        private readonly double scaleY;
        private readonly double offsetX;
        private readonly double offsetY;
        private readonly int maxIterations;
        private readonly double rotationRadians;

        public MandelbrotShader(
            double scaleX,
            double scaleY,
            double offsetX,
            double offsetY,
            int maxIterations = 100,
            double rotationDegrees = 0.0,
            double reflectivity = 0,
            double transparency = 0,
            double indexOfRefraction = 0
        ) : base(reflectivity, transparency, indexOfRefraction)
        {
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.maxIterations = maxIterations;
            rotationRadians = rotationDegrees * (Math.PI / 180.0);
        }

        public override Color Shade(ShadingContext shading, World world)
        {
            double xBase = shading.LocalPosition.X * scaleX + offsetX;
            double zBase = shading.LocalPosition.Z * scaleY + offsetY;

            double cosT = Math.Cos(rotationRadians);
            double sinT = Math.Sin(rotationRadians);

            double xRot = xBase * cosT - zBase * sinT;
            double zRot = xBase * sinT + zBase * cosT;

            double x = 0.0;
            double y = 0.0;
            int iteration = 0;
            for (; iteration < maxIterations; iteration++)
            {
                double x2 = x * x - y * y + xRot;
                double y2 = 2.0 * x * y + zRot;
                x = x2;
                y = y2;

                if (x * x + y * y > 4.0)
                    break;
            }

            if (iteration == maxIterations)
            {
                return new Color(0, 0, 0);
            }
            else
            {
                double t = (double)iteration / maxIterations;
                return new Color(t, t, t);
            }
        }
    }
}

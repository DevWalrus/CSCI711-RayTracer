using RayTracer.Objects;

namespace RayTracer
{
    /// <summary>
    /// Simple color.
    /// </summary>
    public class ColorShader : IMaterial
    {
        public Color baseColor;

        public ColorShader(Color baseColor)
        {
            this.baseColor = baseColor;
        }

        /// <inheritdoc/>
        public Color Shade(Point position, MyVector normal, MyVector viewDir, World world)
        {
            return baseColor;
        }
    }
}

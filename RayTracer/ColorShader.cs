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
        public Color Shade(ShadingContext shading, World world)
        {
            return baseColor;
        }
    }
}

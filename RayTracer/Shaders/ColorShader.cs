using RayTracer.Objects;

namespace RayTracer.Shaders
{
    /// <summary>
    /// Simple color.
    /// </summary>
    public class ColorShader : Material
    {
        public Color baseColor;

        public ColorShader(
            Color baseColor,
            double reflectivity = 0,
            double transparency = 0,
            double indexOfRefraction = 0
        ) : base(reflectivity, transparency, indexOfRefraction)
        {
            this.baseColor = baseColor;
        }

        /// <inheritdoc/>
        public override Color Shade(ShadingContext shading, World world)
        {
            return baseColor;
        }
    }
}

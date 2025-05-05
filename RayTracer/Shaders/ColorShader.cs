using RayTracer.RayMath;
using RayTracer.Objects;

namespace RayTracer.Shaders
{
    /// <summary>
    /// Simple color.
    /// </summary>
    public class ColorShader : Material
    {
        public static ColorShader White = new ColorShader(Color.White);
        public static ColorShader Black = new ColorShader(Color.Black);
        public static ColorShader Red = new ColorShader(Color.Red);
        public static ColorShader Green = new ColorShader(Color.Green);
        public static ColorShader Blue = new ColorShader(Color.Blue);
        public static ColorShader Yellow = new ColorShader(Color.Yellow);
        public static ColorShader SkyBlue = new ColorShader(Color.SkyBlue);
        public static ColorShader Cyan = new ColorShader(Color.Cyan);
        public static ColorShader Magenta = new ColorShader(Color.Magenta);
        public static ColorShader Gray = new ColorShader(Color.Gray);
        public static ColorShader DarkGray = new ColorShader(Color.DarkGray);
        public static ColorShader LightGray = new ColorShader(Color.LightGray);

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

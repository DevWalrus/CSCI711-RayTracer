using RayTracer.RayMath;

namespace RayTracer.Shaders
{
    /// <summary>
    /// Implements a checkerboard material shader that alternates between two colors based on the intersection point.
    /// </summary>
    public class NoisyCheckerboardShader : Material
    {
        private readonly Color _color1;
        private readonly Color _color2;
        private readonly double _squareSize;
        private readonly Perlin _perlin;
        private readonly double _noiseAmplitude;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckerboardShader"/> class with the specified square colors and an optional scale factor.
        /// </summary>
        /// <param name="color1">The color used for one set of squares in the checkerboard pattern.</param>
        /// <param name="color2">The color used for the alternate set of squares in the checkerboard pattern.</param>
        /// <param name="squareSize">The size of each checker square in world units.</param>
        public NoisyCheckerboardShader(
            Color color1, 
            Color color2, 
            double squareSize = 1.0, 
            double noiseAmplitude = 1,
            double reflectivity = 0,
            double transparency = 0,
            double indexOfRefraction = 0
        ) : base(reflectivity, transparency, indexOfRefraction)
        {
            _color1 = color1;
            _color2 = color2;
            _squareSize = squareSize;
            _noiseAmplitude = noiseAmplitude;
            _perlin = new Perlin();
        }

        /// <inheritdoc/>>
        public override Color Shade(ShadingContext shading, World world)
        {
            double u = shading.LocalPosition.X / _squareSize;
            double v = shading.LocalPosition.Z / _squareSize;

            int uCell = (int)Math.Floor(u);
            int vCell = (int)Math.Floor(v);

            bool isEvenCell = (uCell + vCell) % 2 == 0;
            Color baseColor = isEvenCell ? _color1 : _color2;

            var noise = _perlin.Noise(uCell, vCell);

            double factor = 1.0 + (noise - 0.5) * _noiseAmplitude;

            double r = baseColor.R * factor;
            double g = baseColor.G * factor;
            double b = baseColor.B * factor;

            r = Math.Min(Math.Max(r, 0.0), 1.0);
            g = Math.Min(Math.Max(g, 0.0), 1.0);
            b = Math.Min(Math.Max(b, 0.0), 1.0);

            return new Color(r, g, b);
        }
    }
}

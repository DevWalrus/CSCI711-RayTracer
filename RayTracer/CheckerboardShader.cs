using RayTracer.Objects;
using System.Drawing;

namespace RayTracer
{
    /// <summary>
    /// Implements a checkerboard material shader that alternates between two colors based on the intersection point.
    /// </summary>
    public class CheckerboardShader : IMaterial
    {
        private Color _color1 { get; set; }
        private Color _color2 { get; set; }
        private double _squareSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckerboardShader"/> class with the specified square colors and an optional scale factor.
        /// </summary>
        /// <param name="color1">The color used for one set of squares in the checkerboard pattern.</param>
        /// <param name="color2">The color used for the alternate set of squares in the checkerboard pattern.</param>
        /// <param name="squareSize">The size of each checker square in world units.</param>
        public CheckerboardShader(Color color1, Color color2, double squareSize = 1.0)
        {
            this._color1 = color1;
            this._color2 = color2;
            this._squareSize = squareSize;
        }

        /// <inheritdoc/>>
        public Color Shade(ShadingContext shading, World world)
        {
            double u = shading.LocalPosition.X / _squareSize;
            double v = shading.LocalPosition.Z / _squareSize;

            int uCell = (int)Math.Floor(u);
            int vCell = (int)Math.Floor(v);

            bool isEvenCell = ((uCell + vCell) % 2 == 0);
            return isEvenCell ? _color1 : _color2;
        }
    }
}

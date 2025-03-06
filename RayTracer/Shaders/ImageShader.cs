#pragma warning disable CA1416
using System.Drawing;

namespace RayTracer.Shaders
{
    /// <summary>
    /// Implements a checkerboard material shader that alternates between two colors based on the intersection point.
    /// </summary>
    public class ImageShader : IMaterial
    {
        private readonly Bitmap _texture;
        private readonly double _scale;
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckerboardShader"/> class with the specified square colors and an optional scale factor.
        /// </summary>
        /// <param name="color1">The color used for one set of squares in the checkerboard pattern.</param>
        /// <param name="color2">The color used for the alternate set of squares in the checkerboard pattern.</param>
        /// <param name="squareSize">The size of each checker square in world units.</param>
        public ImageShader(string imagePath, double scale = 1.0)
        {
            _texture = new Bitmap(imagePath);
            _scale = scale;
        }

        /// <inheritdoc/>>
        public Color Shade(ShadingContext shading, World world)
        {
            double u = shading.LocalPosition.X / _scale % 1.0;
            double v = shading.LocalPosition.Z / _scale % 1.0;

            if (u < 0) u += 1.0;
            if (v < 0) v += 1.0;

            int xPixel = (int)(u * _texture.Width);
            int yPixel = (int)(v * _texture.Height);

            if (xPixel >= _texture.Width) xPixel = _texture.Width - 1;
            if (yPixel >= _texture.Height) yPixel = _texture.Height - 1;

            System.Drawing.Color pixelColor = _texture.GetPixel(xPixel, yPixel);

            double r = pixelColor.R / 255.0;
            double g = pixelColor.G / 255.0;
            double b = pixelColor.B / 255.0;

            return new Color(r, g, b);
        }
    }
}

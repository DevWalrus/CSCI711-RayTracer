#pragma warning disable CA1416
using RayTracer.RayMath;
using Bitmap = System.Drawing.Bitmap;

namespace RayTracer.Shaders
{
    /// <summary>
    /// Implements a checkerboard material shader that alternates between two colors based on the intersection point.
    /// </summary>
    public class ImageShader : Material
    {
        private Color[,] _pixelBuffer;
        private int width => _pixelBuffer.GetLength(0);
        private int height => _pixelBuffer.GetLength(1);
        private readonly double _scale;
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckerboardShader"/> class with the specified square colors and an optional scale factor.
        /// </summary>
        /// <param name="color1">The color used for one set of squares in the checkerboard pattern.</param>
        /// <param name="color2">The color used for the alternate set of squares in the checkerboard pattern.</param>
        /// <param name="squareSize">The size of each checker square in world units.</param>
        public ImageShader(
            string imagePath, 
            double scale = 1.0,
            double reflectivity = 0,
            double transparency = 0,
            double indexOfRefraction = 0
        ) : base(reflectivity, transparency, indexOfRefraction)
        {
            var bmp = new Bitmap(imagePath);
            _pixelBuffer = new Color[bmp.Width, bmp.Height];
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    var px = bmp.GetPixel(x, y);
                    _pixelBuffer[x, y] = new Color(px.R, px.G, px.B);
                }
            bmp.Dispose(); // release file handle
            _scale = scale;
        }

        /// <inheritdoc/>>
        public override Color Shade(ShadingContext shading, World world)
        {
            double u = shading.U * _scale % 1.0;
            double v = -(shading.V * _scale % 1.0);

            if (u < 0) u += 1.0;
            if (v < 0) v += 1.0;

            int xPixel = (int)(u * width);
            int yPixel = (int)(v * height);

            if (xPixel >= width) xPixel = width - 1;
            if (yPixel >= height) yPixel = height - 1;

            var pixelColor = _pixelBuffer[xPixel, yPixel];

            // right after you read pixelColor.R/G/B:
            double sr = pixelColor.R / 255.0,
                   sg = pixelColor.G / 255.0,
                   sb = pixelColor.B / 255.0;
            // decode to linear:
            double lr = Math.Pow(sr, 2.2),
                   lg = Math.Pow(sg, 2.2),
                   lb = Math.Pow(sb, 2.2);
            return new Color(lr, lg, lb);
        }
    }
}

using System.Drawing;

namespace RayTracer.Shaders
{
    public class BrickShader : Material
    {
        private Color _mortarColor { get; set; }
        private Color _brickColor { get; set; }
        private double _width { get; set; }
        private double _height { get; set; }
        private double _mThickness { get; set; }

        public BrickShader(
            Color mortarColor, 
            Color brickColor, 
            double width,
            double height,
            double mThickness,
            double reflectivity = 0,
            double transparency = 0,
            double indexOfRefraction = 0
        ) : base(reflectivity, transparency, indexOfRefraction)
        {
            _mortarColor = mortarColor;
            _brickColor = brickColor;
            _width = width;
            _height = height;
            _mThickness = mThickness;
        }

        /// <inheritdoc/>>
        public override Color Shade(ShadingContext shading, World world)
        {
            var u = shading.LocalPosition.X / _width;
            var v = shading.LocalPosition.Z / _height;

            var uu = u / (_width + _mThickness);
            var vv = Math.Abs(v / (_height + _mThickness));

            if (uu < 0) uu = Math.Abs(uu) + 1.1;

            if ((uu * 0.5) % 1 > 0.5) vv += 0.5;

            var ubrick = Math.Floor(uu);
            var vbrick = Math.Floor(vv);
            uu -= ubrick;
            vv -= vbrick;

            if ((uu < _mThickness) || (vv < _mThickness))
                return _mortarColor;
            else
                return _brickColor;
        }
    }
}

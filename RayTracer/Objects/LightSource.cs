using System.Drawing;
using MathNet.Numerics.LinearAlgebra;
using RayTracer.RayMath;

namespace RayTracer.Objects
{
    public class LightSource
    {
        private Point _position;
        private Color _color;

        public Point Center { get => _position; }
        public Color Color { get => _color; }

        public LightSource(Point position, Color color)
        {
            _position = position;
            _color = color;
        }

        public void Transform(Matrix<double> m)
        {
            _position.Transform(m);
        }
    }
}

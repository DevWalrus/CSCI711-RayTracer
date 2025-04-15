namespace RayTracer.RayMath
{
    public class Color
    {
        public static Color White = new Color(1, 1, 1);
        public static Color Black = new Color(0, 0, 0);
        public static Color Red = new Color(1, 0, 0);
        public static Color Green = new Color(0, 1, 0);
        public static Color Blue = new Color(0, 0, 1);
        public static Color Yellow = new Color(1, 1, 0);
        public static Color SkyBlue = new Color(0.5, 0.7, 1);
        public static Color Cyan = new Color(0, 1, 1);
        public static Color Magenta = new Color(1, 0, 1);
        public static Color Gray = new Color(0.5, 0.5, 0.5);
        public static Color DarkGray = new Color(0.25, 0.25, 0.25);
        public static Color LightGray = new Color(0.75, 0.75, 0.75);

        public double R;
        public double G;
        public double B;

        public Color(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        public override string ToString()
        {
            return $"R: {R}, G: {G}, B: {B}";
        }

        public override bool Equals(object? obj) => obj is Color other && Equals(other);

        public bool Equals(Color other)
        {
            return Math.Abs(R - other.R) < 1e-6 &&
                Math.Abs(G - other.G) < 1e-6 &&
                Math.Abs(B - other.B) < 1e-6;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static Color operator *(Color m1, double c)
        {
            return new Color(m1.R * c, m1.G * c, m1.B * c);
        }

        public static Color operator *(double c, Color m1)
        {
            return m1 * c;
        }

        public static Color operator +(Color m1, Color m2)
        {
            return new Color(m1.R + m2.R, m1.G + m2.G, m1.B + m2.B);
        }

        public System.Drawing.Color ToSystemColor()
        {
            var r = Math.Min((int)(R * 255), 255);
            var g = Math.Min((int)(G * 255), 255);
            var b = Math.Min((int)(B * 255), 255);
            var sysC = System.Drawing.Color.FromArgb(255, r, g, b);
            return sysC;
        }
    }
}

#pragma warning disable CA1416
using RayTracer.RayMath;
using Bitmap = System.Drawing.Bitmap;

namespace RayTracer
{
    public static class ToneMapper
    {
        public enum ToneOperator { Ward, Reinhard };

        public static Bitmap Map(Color[,] hdr, double Ldmax, ToneOperator op)
        {
            int w = hdr.GetLength(0), h = hdr.GetLength(1);
            const double eps = 1e-4;

            var logSum = 0d;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Color c = hdr[x, y];
                    logSum += Math.Log(eps + (0.27 * c.R + 0.67 * c.G + 0.06 * c.B));
                }
            }
            var Lwa = Math.Exp(logSum / (w * h));

            var ldr = new Bitmap(w, h);
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    var c = hdr[x, y];
                    double R = c.R, G = c.G, B = c.B;
                    double Rt, Gt, Bt;

                    if (op == ToneOperator.Ward)
                    {
                        double num = 1.219 + Math.Pow(Ldmax / 2.0, 0.4);
                        double denom = 1.219 + Math.Pow(Lwa, 0.4);

                        double sf = Math.Pow(
                            num / denom,
                            2.5
                        );
                        Rt = sf * R; Gt = sf * G; Bt = sf * B;
                    }
                    else
                    {
                        var a = 0.18;
                        double Rs = a * R / Lwa, Gs = a * G / Lwa, Bs = a * B / Lwa;
                        double Rr = Rs / (1 + Rs), Gr = Gs / (1 + Gs), Br = Bs / (1 + Bs);
                        Rt = Rr * Ldmax; Gt = Gr * Ldmax; Bt = Br * Ldmax;
                    }

                    var r8 = (byte)(Math.Clamp(Rt / Ldmax, 0, 1) * 255);
                    var g8 = (byte)(Math.Clamp(Gt / Ldmax, 0, 1) * 255);
                    var b8 = (byte)(Math.Clamp(Bt / Ldmax, 0, 1) * 255);

                    ldr.SetPixel(x, y, System.Drawing.Color.FromArgb(r8, g8, b8));
                }

            return ldr;
        }
    }
}

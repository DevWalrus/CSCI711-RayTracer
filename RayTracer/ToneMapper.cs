#pragma warning disable CA1416
using System.Drawing;
using Bitmap = System.Drawing.Bitmap;

namespace RayTracer
{
    public static class ToneMapper
    {
        public enum ToneOperator { Ward, Reinhard };

        public static Bitmap Map(Bitmap hdr, double Ldmax, ToneOperator op)
        {
            int w = hdr.Width, h = hdr.Height;
            const double δ = 1e-4;

            // 1) compute log‐average luminance
            double logSum = 0;
            double[,] L = new double[w, h];
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    Color c = hdr.GetPixel(x, y);
                    double R = c.R / 255.0, G = c.G / 255.0, B = c.B / 255.0;
                    L[x, y] = 0.27 * R + 0.67 * G + 0.06 * B;
                    logSum += Math.Log(δ + L[x, y]);
                }
            double Lwa = Math.Exp(logSum / (w * h));

            // 2) produce a fresh LDR bitmap
            var ldr = new Bitmap(w, h, hdr.PixelFormat);
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    Color c = hdr.GetPixel(x, y);
                    double R = c.R / 255.0, G = c.G / 255.0, B = c.B / 255.0;
                    double Rt, Gt, Bt;

                    if (op == ToneOperator.Ward)
                    {
                        double sf = Math.Pow(
                            (1.219 + Ldmax / 2) / (1.219 + Lwa),
                            2.5
                        );
                        Rt = sf * R; Gt = sf * G; Bt = sf * B;
                    }
                    else
                    {
                        // Reinhard
                        double a = 0.18;
                        double Rs = a * R / Lwa, Gs = a * G / Lwa, Bs = a * B / Lwa;
                        double Rr = Rs / (1 + Rs), Gr = Gs / (1 + Gs), Br = Bs / (1 + Bs);
                        Rt = Rr * Ldmax; Gt = Gr * Ldmax; Bt = Br * Ldmax;
                    }

                    // normalize into [0,1]
                    byte r8 = (byte)(Math.Clamp(Rt / Ldmax, 0, 1) * 255);
                    byte g8 = (byte)(Math.Clamp(Gt / Ldmax, 0, 1) * 255);
                    byte b8 = (byte)(Math.Clamp(Bt / Ldmax, 0, 1) * 255);

                    ldr.SetPixel(x, y, Color.FromArgb(r8, g8, b8));
                }

            return ldr;
        }
    }
}

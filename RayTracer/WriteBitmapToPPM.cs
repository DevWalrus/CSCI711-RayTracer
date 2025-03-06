#pragma warning disable CA1416
using System.Drawing;
using System.Text;

namespace RayTracer
{
    public class PPMWriter
    {
        public static void WriteBitmapToPPM(string file, Bitmap bitmap)
        {
            //Use a streamwriter to write the text part of the encoding
            string header = $"P6\n{bitmap.Width} {bitmap.Height}\n255\n";
            File.WriteAllBytes(file, Encoding.ASCII.GetBytes(header));
            //Switch to a binary writer to write the data
            using (var fs = new FileStream(file, FileMode.Append))
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        System.Drawing.Color color = bitmap.GetPixel(x, y);
                        fs.WriteByte(color.R);
                        fs.WriteByte(color.G);
                        fs.WriteByte(color.B);
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Quantizacao
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bitmap = PPM.ReadToBitmap("C:/Users/Eduardo/Desktop/PedraFurada.ppm");
            bitmap = PPM.Quantize(bitmap, 50);
            PPM.WriteFromBitmap("C:/Users/Eduardo/Desktop/PedraFuradaRes.ppm", bitmap);
            HashSet<Color> cores = new HashSet<Color>();
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    cores.Add(Color.FromArgb(pixel.R, pixel.G, pixel.B));
                }
            }
            Console.WriteLine(cores.Count);
        }
    }
}

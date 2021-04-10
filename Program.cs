using System;
using System.Collections.Generic;
using System.Drawing;

namespace Quantizacao
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bitmap = PPM.ReadToBitmap("C:/Users/Eduardo/Desktop/Colegio.ppm");
            bitmap = PPM.Quantize(bitmap, 50);
            PPM.WriteFromBitmap("C:/Users/Eduardo/Desktop/ColegioRes.ppm", bitmap);
        }
    }
}

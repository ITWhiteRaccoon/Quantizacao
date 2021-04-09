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
        }
    }
}

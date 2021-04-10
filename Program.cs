using System;
using System.Drawing;

namespace Quantizacao
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("usage: quantize <inputImage> <outputImage> <numerOfColors>");
            }

            string inputImage = args[0].Trim(), outputImage = args[1].Trim();
            int numberOfColors = Convert.ToInt32(args[2].Trim());

            Bitmap bitmap = PPM.ReadToBitmap(inputImage);
            bitmap = PPM.Quantize(bitmap, numberOfColors);
            PPM.WriteFromBitmap(outputImage, bitmap);
        }
    }
}

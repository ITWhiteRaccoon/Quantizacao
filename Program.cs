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
                Console.WriteLine("usage: Quantizacao <inputImage> <outputImage> <numerOfColors>");
                return;
            }

            string inputImage = args[0].Trim(), outputImage = args[1].Trim();
            int numberOfColors = Convert.ToInt32(args[2].Trim());

            Console.WriteLine($"=== {"",5}Lendo Arquivo{"",5} ===");
            Bitmap bitmap = PPM.ReadToBitmap(inputImage);

            Console.WriteLine($"=== {"",6}Quantizando{"",6} ===");
            bitmap = PPM.Quantize(bitmap, numberOfColors);

            Console.WriteLine($"=== {"",3}Gravando Arquivo{"",4} ===");
            PPM.WriteFromBitmap(outputImage, bitmap);
        }
    }
}

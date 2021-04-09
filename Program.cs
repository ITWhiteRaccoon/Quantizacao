using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Quantizacao
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bmp = ReadBitmapFromPPM("data/Colegio.ppm");
            WriteBitmapToPPM("C:/Users/Eduardo/Desktop/teste.ppm", bmp);
        }

        //public static Dictionary<>
        public static Bitmap ReadBitmapFromPPM(string file)
        {
            Bitmap bmp = null;
            var reader = new StreamReader(file);
            string line, ppmType = "";
            int width = -1, height = -1, colorLimit = -1;
            while (ppmType.Length == 0 || width == -1 || height == -1 || colorLimit == -1)
            {
                line = reader.ReadLine();
                if (line == null || (line = line.Trim()).Length <= 0 || line[0] == '#')
                {
                    continue;
                }

                Match match;

                if (ppmType.Length == 0)
                {
                    match = Regex.Match(line, @"^P\d$");
                    if (match.Captures.Count >= 1)
                    {
                        if (match.Captures[0].Value != "P3")
                        {
                            return null;
                        }

                        ppmType = "P3";
                    }
                }

                if (width == -1 || height == -1)
                {
                    match = Regex.Match(line, @"^\d+\s+\d+$");
                    if (match.Captures.Count >= 1)
                    {
                        string[] data = match.Captures[0].Value.Split((char[])null,
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        width = Convert.ToInt32(data[0]);
                        height = Convert.ToInt32(data[1]);
                    }
                }

                if (colorLimit == -1)
                {
                    match = Regex.Match(line, @"^\d+$");
                    if (match.Captures.Count >= 1)
                    {
                        colorLimit = Convert.ToInt32(match.Captures[0].Value);
                    }
                }
            }

            var y = 0;
            bmp = new Bitmap(width, height);
            while ((line = reader.ReadLine()) != null && (line = line.Trim()).Length >= 0 && line[0] != '#')
            {
                string[] colors = line.Split((char[])null,
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (colors.Length != width * 3) { return null; }


                for (var x = 0; x < colors.Length / 3; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(
                        Convert.ToInt32(colors[x * 3]),
                        Convert.ToInt32(colors[x * 3 + 1]),
                        Convert.ToInt32(colors[x * 3 + 2])
                    ));
                }

                y++;
            }

            return bmp;
        }


        public static void WriteBitmapToPPM(string file, Bitmap bitmap)
        {
            var writer = new StreamWriter(file);
            writer.WriteLine("P3");
            writer.WriteLine($"{bitmap.Width} {bitmap.Height}");
            writer.WriteLine("255");
            for (var y = 0;
                y < bitmap.Height;
                y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    writer.Write($" {color.R} {color.G} {color.B}");
                }

                writer.WriteLine();
            }

            writer.Close();
        }
    }
}

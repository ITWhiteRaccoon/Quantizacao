using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Quantizacao
{
    public static class PPM
    {
        private class ColorMap : IComparable<ColorMap>
        {
            public int Frequency;
            public int R;
            public int G;
            public int B;

            public int CompareTo(ColorMap other)
            {
                return other == null ? 1 : Frequency.CompareTo(other.Frequency);
            }

            public double RelDistance(ColorMap other)
            {
                return (other.R - R) * (other.R - R) + (other.G - G) * (other.G - G) + (other.B - B) * (other.B - B);
            }

            public double RelDistance(Color other)
            {
                return (other.R - R) * (other.R - R) + (other.G - G) * (other.G - G) + (other.B - B) * (other.B - B);
            }
        }

        public static Bitmap Quantize(Bitmap bitmap, int nOfColors)
        {
            var colorChart = new Dictionary<Color, ColorMap>();
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    if (colorChart.ContainsKey(pixel))
                    {
                        ColorMap map = colorChart[pixel];
                        map.Frequency++;
                    }
                    else
                    {
                        colorChart[pixel] = new ColorMap {Frequency = 1, R = pixel.R, G = pixel.G, B = pixel.B};
                    }
                }
            }

            List<ColorMap> mostFrequent = colorChart.Values.Select(color => new ColorMap()
                {Frequency = color.Frequency, R = color.R, G = color.G, B = color.B}).ToList();

            mostFrequent = mostFrequent.OrderByDescending(i => i).ToList();
            int baseI = 0;
            while (mostFrequent.Count > nOfColors)
            {
                if (baseI >= mostFrequent.Count - 2)
                {
                    baseI = 0;
                }

                ColorMap baseColor = mostFrequent[baseI];
                ColorMap closestColor = mostFrequent[baseI + 1];
                double colorDistance = baseColor.RelDistance(closestColor);

                for (int i = baseI + 2; i < mostFrequent.Count; i++)
                {
                    double newDistance;
                    if ((newDistance = baseColor.RelDistance(mostFrequent[i])) < colorDistance)
                    {
                        colorDistance = newDistance;
                        closestColor = mostFrequent[i];
                    }
                }

                mostFrequent.Remove(closestColor);
                baseI++;
            }

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color currentPixel = bitmap.GetPixel(x, y);
                    bool isListed = false;
                    ColorMap closest = mostFrequent[0];
                    double closestDistance = closest.RelDistance(currentPixel);
                    foreach (ColorMap color in mostFrequent)
                    {
                        if (currentPixel.R == color.R &&
                            currentPixel.G == color.G &&
                            currentPixel.B == color.B)
                        {
                            isListed = true;
                            break;
                        }

                        double newDistance;
                        if ((newDistance = color.RelDistance(currentPixel)) < closestDistance)
                        {
                            closest = color;
                            closestDistance = newDistance;
                        }
                    }

                    if (!isListed)
                    {
                        bitmap.SetPixel(x, y, Color.FromArgb(closest.R, closest.G, closest.B));
                    }

                    { }
                }
            }

            return bitmap;
        }

        #region IO

        public static Bitmap ReadToBitmap(string file)
        {
            Bitmap bitmap = null;
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
                        string[] data = match.Captures[0].Value.Split((char[]) null,
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

            int y = 0;
            bitmap = new Bitmap(width, height);
            while ((line = reader.ReadLine()) != null && (line = line.Trim()).Length >= 0 && line[0] != '#')
            {
                string[] colors = line.Split((char[]) null,
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (colors.Length != width * 3) { return null; }


                for (int x = 0; x < colors.Length / 3; x++)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(
                        Convert.ToInt32(colors[x * 3]),
                        Convert.ToInt32(colors[x * 3 + 1]),
                        Convert.ToInt32(colors[x * 3 + 2])
                    ));
                }

                y++;
            }

            return bitmap;
        }


        public static void WriteFromBitmap(string file, Bitmap bitmap)
        {
            var writer = new StreamWriter(file);
            writer.WriteLine("P3");
            writer.WriteLine($"{bitmap.Width} {bitmap.Height}");
            writer.WriteLine("255");
            for (int y = 0;
                y < bitmap.Height;
                y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    writer.Write($" {color.R} {color.G} {color.B}");
                }

                writer.WriteLine();
            }

            writer.Close();
        }

        #endregion
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageToASCII
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string brightnessTableSimple = " .:-=+*#%@";
            string brightnessTableComplex = " .'`^\",:;Il!i><~+_-?][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";
            bool isComplexTableUsed;

            if (args.Length == 0)
            {
                Console.WriteLine("No file given");
                Console.ReadLine();
                return;
            }

            Bitmap rawImage = new Bitmap(args[0]);

            Console.WriteLine("Use simple or complex brightness table? (s/c)");
            string input = Console.ReadLine();
            if (input == "s")
                isComplexTableUsed = false;
            else if (input == "c")
                isComplexTableUsed = true;
            else
            {
                Console.WriteLine("Invalid input");
                Console.ReadLine();
                return;
            }
            Console.Clear();

            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Bitmap resizedImage = ResizeImage(rawImage, CalculateImageNewSize(rawImage));
            rawImage.Dispose();

            for (int y = 0; y < resizedImage.Height; y++)
            {
                for  (int x = 0; x < resizedImage.Width; x++)
                {
                    Color pixelColor = resizedImage.GetPixel(x, y);
                    //int pixelAverageValue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    int pixelAverageValue = (int)Math.Round(pixelColor.R * 0.299f + pixelColor.G * 0.587f + pixelColor.B * 0.114f);
                    resizedImage.SetPixel(x, y, Color.FromArgb(pixelAverageValue, pixelAverageValue, pixelAverageValue));
                }
            }
            for (int y = 0; y < resizedImage.Height; y++) 
            {
                for (int x = 0; x < resizedImage.Width; x++)  {
                    Color pixelColor = resizedImage.GetPixel(x, y);
                    if (isComplexTableUsed)
                    {
                        int temp = (int)Math.Round(pixelColor.GetBrightness() * 69);
                        Console.Write(string.Concat(brightnessTableComplex[temp], brightnessTableComplex[temp]));
                    } else
                    {
                        int temp = (int)Math.Round(pixelColor.GetBrightness() * 9);
                        Console.Write(string.Concat(brightnessTableSimple[temp], brightnessTableSimple[temp]));
                    }
                }
                Console.Write("\n");
            }
            resizedImage.Dispose();

            Console.ReadLine();
        }
        public static Size CalculateImageNewSize(Bitmap image)
        {
            int newX = image.Width;
            int newY = image.Height;
            float factor;
            if (newX > Console.WindowWidth/2)
            {
                factor = (float)Console.WindowWidth / (2*newX);
                newX = (int)(newX * factor);
                newY = (int)(newY * factor);
            }
            if (newY > Console.WindowHeight)
            {
                factor = (float)Console.WindowHeight / newY;
                newX = (int)(newX * factor);
                newY = (int)(newY * factor);
            }
            return new Size(newX, newY);
        }
        public static Bitmap ResizeImage(Image image, Size size)
        {
            var destRect = new Rectangle(0, 0, size.Width, size.Height);
            var destImage = new Bitmap(size.Width, size.Height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
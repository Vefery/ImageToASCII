using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageToASCII
{
    internal class Program
    {
        readonly static string brightnessTable = " .:-=+*#%@";
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No file given. Drag and drop image on .exe file");
                Console.ReadLine();
                return;
            }

            Bitmap image = new Bitmap(args[0]);

            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            image = ResizeImage(image, CalculateImageNewSize(image));

            DisplayImageAsASCII(image);

            image.Dispose();

            Console.ReadLine();
        }
        public static Size CalculateImageNewSize(Bitmap image)
        {
            int newX = image.Width;
            int newY = image.Height;
            float factor;
            if (newX > Console.WindowWidth)
            {
                factor = (float)Console.WindowWidth / newX;
                newX = (int)(newX * factor);
                newY = (int)(newY * factor);
            }
            if (newY/2 > Console.WindowHeight)
            {
                factor = (float)Console.WindowHeight / (newY * 0.5f);
                newX = (int)(newX * factor);
                newY = (int)(newY * 0.5f * factor);
            }
            return new Size(newX, newY);
        }
        public static Bitmap ResizeImage(Bitmap image, Size size)
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
        public static void DisplayImageAsASCII(Bitmap image)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int brightness = (int)(pixelColor.R * 0.299f + pixelColor.G * 0.587f + pixelColor.B * 0.114f) / 3;
                    int temp = (int)Math.Round(((float)(brightnessTable.Length - 1) / 85) * brightness);
                    Console.Write(brightnessTable[temp]);
                }
                Console.Write("\n");
            }
        }
    }
}
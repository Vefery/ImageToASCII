using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

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
            Size newSize = CalculateImageNewSize(image);

            FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);
            int framesCount = image.GetFrameCount(dimension);
            Bitmap[] sequence = BuildImageSequence(image, newSize, dimension, framesCount);
            image.Dispose();

            for (int i = 0; i < framesCount; i++)
            {
                DisplayImageAsASCII(sequence[i]);
                System.Threading.Thread.Sleep(60);
            }

            foreach (var s in sequence)
            {
                s.Dispose();
            }
            Console.ReadLine();
        }

        private static Bitmap[] BuildImageSequence(Bitmap image, Size newSize, FrameDimension dimension, int framesCount)
        {
            Bitmap[] sequence = new Bitmap[framesCount];
            for (int i = 0; i < framesCount; i++)
            {
                sequence[i] = new Bitmap(image, newSize);
                image.SelectActiveFrame(dimension, i);
            }
            return sequence;
        }

        public static Size CalculateImageNewSize(Image image)
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
        public static void DisplayImageAsASCII(Bitmap image)
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            using (var stream = new StreamWriter(Console.OpenStandardOutput()))
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Color pixelColor = image.GetPixel(x, y);
                        int brightness = (int)(pixelColor.R * 0.299f + pixelColor.G * 0.587f + pixelColor.B * 0.114f) / 3;
                        int temp = (int)Math.Round(((float)(brightnessTable.Length - 1) / 85) * brightness);
                        //if (buffer[])
                        stream.Write(brightnessTable[temp]);
                    }
                    stream.Write("\n");
                }
            }
        }
    }
}
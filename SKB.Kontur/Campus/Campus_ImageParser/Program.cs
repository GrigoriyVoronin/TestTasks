using System;
using System.IO;

namespace ImageParser
{
    internal class Program
    {
        public static void Main()
        {
            var parser = new ImageParser();
            string imageInfoJson;

            using (var file = new FileStream("image.png", FileMode.Open, FileAccess.Read))
            {
                imageInfoJson = parser.GetImageInfo(file);
            }

            Console.WriteLine(imageInfoJson);

            using (var file = new FileStream("image1.bmp", FileMode.Open, FileAccess.Read))
            {
                imageInfoJson = parser.GetImageInfo(file);
            }

            Console.WriteLine(imageInfoJson);

            using (var file = new FileStream("image2.gif", FileMode.Open, FileAccess.Read))
            {
                imageInfoJson = parser.GetImageInfo(file);
            }

            Console.WriteLine(imageInfoJson);
        }
    }
}
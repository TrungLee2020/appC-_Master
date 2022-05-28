using System;
using System.Linq;
using System.Text;


namespace MainWindow.ImageHelpers
{
    public static class ImageValidation
    {
        public static bool IsValidImage(this byte[] image)
        {
            var imageFormat = GetImageFormat(image);
            return imageFormat == ImageFormat.jpeg ||
                imageFormat == ImageFormat.png;
        }

        public enum ImageFormat
        {
            bmp,
            jpeg,
            png,
            unknown
        }

        public static ImageFormat GetImageFormat(byte[] bytes)
        {
            var bmp = Encoding.ASCII.GetBytes("BM");
            var jpeg = new byte[] { 255, 216, 255, 224 };
            var png = new byte[] { 137, 80, 78, 71 };
            var jpeg2 = new byte[] { 255, 216, 255, 225 };

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return ImageFormat.bmp;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return ImageFormat.jpeg;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return ImageFormat.png;

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return ImageFormat.jpeg;

            return ImageFormat.unknown;

        }
    }
}

using System.Windows.Media;

namespace QuAnalyzer.Generic.Extensions
{
    public static class ColorExtensions
    {

        public static byte[] AsByteArray(this Color src)
        {
            return new byte[] { src.R, src.G, src.B };
        }

        public static System.Drawing.Color AsDrawingColor(this Color src)
        {
            return System.Drawing.Color.FromArgb(src.R, src.G, src.B);
        }


    }
}

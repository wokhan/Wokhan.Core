using System.Linq;

namespace Wokhan.Core.Extensions
{
    public static class ValueTupleExtensions
    {
        public static void Deconstruct<T>(this T[] src, out T x1, out T x2)
        {
            x1 = src.ElementAtOrDefault(0);
            x2 = src.ElementAtOrDefault(1);
        }


        public static void Deconstruct<T>(this T[] src, out T x1, out T x2, out T x3)
        {
            x1 = src.ElementAtOrDefault(0);
            x2 = src.ElementAtOrDefault(1);
            x3 = src.ElementAtOrDefault(2);
        }

        public static void Deconstruct<T>(this T[] src, out T x1, out T x2, out T x3, out T x4)
        {
            x1 = src.ElementAtOrDefault(0);
            x2 = src.ElementAtOrDefault(1);
            x3 = src.ElementAtOrDefault(2);
            x4 = src.ElementAtOrDefault(4);
        }

    }
}
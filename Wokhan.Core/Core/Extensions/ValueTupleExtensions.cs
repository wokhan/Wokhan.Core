using System.Collections.Generic;
using System.Linq;

namespace Wokhan.Core.Extensions
{
    public static class ValueTupleExtensions
    {
        public static void Deconstruct<T>(this IEnumerable<T> src, out T x1, out T x2)
        {
            x1 = src.ElementAtOrDefault(0);
            x2 = src.ElementAtOrDefault(1);
        }


        public static void Deconstruct<T>(this IEnumerable<T> src, out T x1, out T x2, out T x3)
        {
            Deconstruct(src, out x1, out x2);
            x3 = src.ElementAtOrDefault(2);
        }

        public static void Deconstruct<T>(this IEnumerable<T> src, out T x1, out T x2, out T x3, out T x4)
        {
            Deconstruct(src, out x1, out x2, out x3);
            x4 = src.ElementAtOrDefault(4);
        }

        public static void Deconstruct<T>(this IEnumerable<T> src, out T x1, out T x2, out T x3, out T x4, out T x5)
        {
            Deconstruct(src, out x1, out x2, out x3, out x4);
            x5 = src.ElementAtOrDefault(5);
        }

    }
}
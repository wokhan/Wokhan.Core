using System.Collections.Generic;
using System.Linq;

namespace Wokhan.Core.Extensions
{
    /// <summary>
    /// Extension methods for ValueTuples
    /// </summary>
    public static class ValueTupleExtensions
    {
        /// <summary>
        /// Projects each elements of the specified <see cref="IEnumerable{T}"/> to out parameters.
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source enumeration</param>
        /// <param name="x1">Variable to map the first item to</param>
        /// <param name="x2">Variable to map the second item to</param>
        public static void Deconstruct<T>(this IEnumerable<T> src, out T x1, out T x2)
        {
            x1 = src.ElementAtOrDefault(0);
            x2 = src.ElementAtOrDefault(1);
        }


        /// <summary>
        /// Projects each elements of the specified <see cref="IEnumerable{T}"/> to out parameters.
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source enumeration</param>
        /// <param name="x1">Variable to map the first item to</param>
        /// <param name="x2">Variable to map the second item to</param>
        /// <param name="x3">Variable to map the third item to</param>
        public static void Deconstruct<T>(this IEnumerable<T> src, out T x1, out T x2, out T x3)
        {
            Deconstruct(src, out x1, out x2);
            x3 = src.ElementAtOrDefault(2);
        }

        /// <summary>
        /// Projects each elements of the specified <see cref="IEnumerable{T}"/> to out parameters.
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source enumeration</param>
        /// <param name="x1">Variable to map the first item to</param>
        /// <param name="x2">Variable to map the second item to</param>
        /// <param name="x3">Variable to map the third item to</param>
        /// <param name="x4">Variable to map the fourth item to</param>
        public static void Deconstruct<T>(this IEnumerable<T> src, out T x1, out T x2, out T x3, out T x4)
        {
            Deconstruct(src, out x1, out x2, out x3);
            x4 = src.ElementAtOrDefault(4);
        }

        /// <summary>
        /// Projects each elements of the specified <see cref="IEnumerable{T}"/> to out parameters.
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source enumeration</param>
        /// <param name="x1">Variable to map the first item to</param>
        /// <param name="x2">Variable to map the second item to</param>
        /// <param name="x3">Variable to map the third item to</param>
        /// <param name="x4">Variable to map the fourth item to</param>
        /// <param name="x5">Variable to map the fifth item to</param>
        public static void Deconstruct<T>(this IEnumerable<T> src, out T x1, out T x2, out T x3, out T x4, out T x5)
        {
            Deconstruct(src, out x1, out x2, out x3, out x4);
            x5 = src.ElementAtOrDefault(5);
        }

    }
}
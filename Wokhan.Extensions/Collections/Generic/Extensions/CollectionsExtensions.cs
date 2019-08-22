using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Wokhan.Collections.Generic.Extensions
{
    public static class CollectionsExtensions
    {
        public static IList<T> AddRange<T>(this IList<T> src, IEnumerable<T> items)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            if (items == null)
            {
                return src;
            }

            foreach (T item in items)
            {
                src.Add(item);
            }

            return src;
        }

        public static IList AddRange(this IList src, IEnumerable items)
        {
            foreach (var item in items)
            {
                src.Add(item);
            }

            return src;
        }

        public static IList<T> RemoveRange<T>(this IList<T> src, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                src.Remove(item);
            }

            return src;
        }


        public static IList RemoveRange(this IList src, IEnumerable items)
        {
            foreach (var item in items)
            {
                src.Remove(item);
            }

            return src;
        }

        public static void InsertOrdered<T, TK>(this IList<T> src, T value, TK orderDet, Func<T, TK> orderDetCib, bool distinct = false) where TK : IComparable
        {
            if (src.Count == 0)
            {
                src.Add(value);
            }
            else
            {
                var pos = src.Select((s, i) => new { s, i }).SkipWhile(s => orderDetCib(s.s).CompareTo(orderDet) == -1).DefaultIfEmpty(null).First()?.i ?? src.Count - 1;
                if (!distinct || !value.Equals(src[pos]))
                {
                    src.Insert(pos, value);
                }
            }
        }
    }
}

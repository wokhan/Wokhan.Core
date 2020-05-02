using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Wokhan.Collections.Generic.Extensions
{
    /// <summary>
    /// Extensions for System.Collections.Generic types
    /// </summary>
    public static class CollectionsExtensions
    {
        /// <summary>
        /// Deprecated: replaced by <see cref="EnumerableExtensions.AddAll{T}(ICollection{T}, IEnumerable{T})"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        [Obsolete("Replaced by EnumerableExtensions.AddAll")]
        public static IList<T> AddRange<T>(this IList<T> src, IEnumerable<T> items)
        {
            Contract.Requires(src != null);

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

        /// <summary>
        /// Deprecated: replaced by <see cref="EnumerableExtensions.AddAll{T}(ICollection{T}, IEnumerable{T})"/>.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        [Obsolete("Replaced by EnumerableExtensions.AddAll")]
        public static IList AddRange(this IList src, IEnumerable items)
        {
            Contract.Requires(src != null);
            Contract.Requires(items != null);

            foreach (var item in items)
            {
                src.Add(item);
            }

            return src;
        }

        /// <summary>
        /// Removes a collection of items from another collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="items">The items to remove</param>
        /// <returns></returns>
        public static ICollection<T> RemoveRange<T>(this ICollection<T> src, IEnumerable<T> items)
        {
            Contract.Requires(src != null);
            Contract.Requires(items != null);

            foreach (T item in items)
            {
                src.Remove(item);
            }

            return src;
        }

        /// <summary>
        /// Removes a collection of items of a given IList.
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <param name="items">Items to remove</param>
        /// <returns></returns>
        public static IList RemoveRange(this IList src, IEnumerable items)
        {
            Contract.Requires(src != null);
            Contract.Requires(items != null);

            foreach (var item in items)
            {
                src.Remove(item);
            }

            return src;
        }

        /// <summary>
        /// Insert an element into an IList following the specified ordering selector.
        /// Note: to be retested. Please don't trust this code as no automated test exists and I think it's wrongly designed...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TK"></typeparam>
        /// <param name="src"></param>
        /// <param name="value"></param>
        /// <param name="orderDet"></param>
        /// <param name="orderDetCib"></param>
        /// <param name="distinct"></param>
        public static void InsertOrdered<T, TK>(this IList<T> src, T value, TK orderDet, Func<T, TK> orderDetCib, bool distinct = false) where TK : IComparable
        {
            Contract.Requires(src != null);
            Contract.Requires(orderDetCib != null);

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

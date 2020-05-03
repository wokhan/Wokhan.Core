using System;
using System.Collections.Generic;
using System.Linq;

namespace Wokhan.Collections.Generic.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets all items from a dictionary given a collection of keys.
        /// If a value is not found, default(TValue) is returned.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="src">Source dictionary</param>
        /// <param name="keys">List of keys to retrieve values for</param>
        /// <returns></returns>
        public static IEnumerable<TValue> GetValuesOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> src, params TKey[] keys)
        {
            return keys.Select(key => src.TryGetValue(key, out TValue val) ? val : default);
        }

        /// <summary>
        /// Flattens a dictionary (concatenating the keys using the specified separator, or "." if none.
        /// </summary>
        /// <param name="src">Source Dictionary (as a <see cref="IEnumerable{T}"/>)</param>
        /// <param name="parentKey">Initial key</param>
        /// <param name="separator">Keys separator (default: ".")</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<object, object>> Flatten(this IEnumerable<KeyValuePair<object, object>> src, string parentKey = "", string separator = ".")
        {
            if (src != null)
            {
                return src.SelectMany(entry =>
                {
                    if (entry.Value is IEnumerable<KeyValuePair<object, object>>)
                    {
                        return ((IEnumerable<KeyValuePair<object, object>>)entry.Value).Flatten($"{parentKey}{separator}{entry.Key}");
                    }
                    else if (entry.Value is IList<object>)
                    {
                        return ((IList<object>)entry.Value).OfType<IEnumerable<KeyValuePair<object, object>>>()
                                                           .SelectMany((e, i) => e.Flatten($"{parentKey}.{entry.Key}[{i}]"))
                                                           .DefaultIfEmpty(new KeyValuePair<object, object>($"{parentKey}{separator}{entry.Key}", String.Join(",", ((IList<object>)entry.Value).Select(e => e.ToString()).OrderBy(e => e))));
                    }
                    else
                    {
                        return new[] { new KeyValuePair<object, object>($"{parentKey}{separator}{entry.Key}", entry.Value) };
                    }
                });
            }
            else
            {
                return Array.Empty<KeyValuePair<object, object>>();
            }
        }

        /*public static IEnumerable<T> Flatten<T>(this IEnumerable<T> d, Func<T, string> getTitle, Func<T, IEnumerable<T>> getChildren, string parentKey = "")
        {
            if (d != null)
            {
                return d.SelectMany(entry =>
                {
                    var val = getChildren(entry);
                    var key = getTitle(entry);
                    if (val is IEnumerable<T>)
                    {
                        return ((IEnumerable<T>)val).Flatten(getTitle, getChildren, parentKey + "." + getTitle(entry));
                    }
                    else if (val is IList<object>)
                    {
                        return ((IList<object>)val).SelectMany((e, i) => (e as IEnumerable<T>)?.Flatten(getTitle, getChildren, $"{parentKey}.{key}[{i}]") 
                                                                      ?? new new KeyValuePair<object, object>($"{parentKey}.{key}", String.Join(",", ((IList<object>)val).OrderBy(e => e))));
                    }
                    else
                    {
                        return new[] { new KeyValuePair<object, object>(parentKey + "." + key, val) };
                    }
                });
            }
            else
            {
                return Array.Empty<T>();
            }
        }*/
    }
}
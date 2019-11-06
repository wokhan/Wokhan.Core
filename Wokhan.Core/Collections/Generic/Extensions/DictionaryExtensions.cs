using System;
using System.Collections.Generic;
using System.Linq;

namespace Wokhan.Collections.Generic.Extensions
{
    public static class DictionaryExtensions
    {

        public static IEnumerable<TValue> GetValuesOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> src, params TKey[] keys)
        {
            return keys.Select(key => src.TryGetValue(key, out TValue val) ? val : default(TValue));
        }

        public static IEnumerable<KeyValuePair<object, object>> Flatten(this IEnumerable<KeyValuePair<object, object>> d, string parentKey = "")
        {
            if (d != null)
            {
                return d.SelectMany(entry =>
                {
                    if (entry.Value is IEnumerable<KeyValuePair<object, object>>)
                    {
                        return ((IEnumerable<KeyValuePair<object, object>>)entry.Value).Flatten(parentKey + "." + entry.Key);
                    }
                    else if (entry.Value is IList<object>)
                    {
                        return ((IList<object>)entry.Value).OfType<IEnumerable<KeyValuePair<object, object>>>()
                                                           .SelectMany((e, i) => e.Flatten($"{parentKey}.{entry.Key}[{i}]"))
                                                           .DefaultIfEmpty(new KeyValuePair<object, object>(parentKey + "." + entry.Key, String.Join(",", ((IList<object>)entry.Value).Select(e => e.ToString()).OrderBy(e => e))));
                    }
                    else
                    {
                        return new[] { new KeyValuePair<object, object>(parentKey + "." + entry.Key, entry.Value) };
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
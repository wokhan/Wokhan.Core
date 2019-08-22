using System;
using System.Collections.Generic;
using System.Linq;

namespace Wokhan.Collections.Generic.Extensions
{
    public static class DictionaryExtensions
    {
        private static List<KeyValuePair<object, object>> emptyList = new List<KeyValuePair<object, object>>();
        public static IEnumerable<KeyValuePair<object, object>> Flatten(this IDictionary<object, object> d, string parentKey = "")
        {
            {
                if (d != null)
                {
                    return d.SelectMany(entry =>
                    {
                        if (entry.Value is IDictionary<object, object>)
                        {
                            return ((IDictionary<object, object>)entry.Value).Flatten(parentKey + "." + entry.Key);
                        }
                        else if (entry.Value is IList<object>)
                        {
                            return ((IList<object>)entry.Value).OfType<IDictionary<object, object>>()
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
                    return emptyList;
                }
            }

        }
    }
}
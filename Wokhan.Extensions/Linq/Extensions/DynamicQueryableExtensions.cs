using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Wokhan.Linq.Extensions
{
    public static class DynamicQueryableExtensions
    {
        public static IQueryable AggregateBy(this IQueryable src, IList<string> members, IDictionary<string, string> aggregateOperation)
        {
            if (members.Any() && aggregateOperation.Any())
            {
                return src.GroupBy("new(" + String.Join(",", members) + ")", "it")
                      .Select("new(it.Key." + String.Join(",it.Key.", members) + "," + String.Join(",", aggregateOperation.Select(c => String.Format(c.Value, c.Key) + " as " + c.Key)) + ")");
            }

            return src;
        }

    }
}

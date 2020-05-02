using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Wokhan.Linq.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IQueryable{T}"/> objects (extending <see cref="System.Linq.Dynamic.Core.DynamicQueryableExtensions"/> library)
    /// </summary>
    public static class DynamicQueryableExtensions
    {
        /// <summary>
        /// Orders a queryable by all specified sorters (passed as property names)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src">Source IQueryable to apply ordering to</param>
        /// <param name="sorters">Enumeration of property to sort on (to sort by descending order, add a "-" at the end of the property name)</param>
        /// <returns>Returns a <see cref="IOrderedQueryable{T}"/></returns>
        public static IOrderedQueryable<T> OrderByMany<T>(this IQueryable<T> src, IEnumerable<string> sorters)
        {
            var initSorter = sorters.First().Trim('-');

            var ret = sorters.First().EndsWith("-") ? src.OrderBy(initSorter + " descending") : src.OrderBy(initSorter);
            foreach (var attr in sorters.Skip(1))
            {
                ret = ret.ThenBy(attr.EndsWith("-") ? attr.Trim('-') + " descending" : attr);
            }

            return ret;
        }
        
        /// <summary>
        /// Performs multiple agregations dynamically, returning a queryable collection of dynamic objects which 
        /// properties defined by the <paramref name="members"/> list, 
        /// along with new properties computed using <paramref name="aggregateOperation"/>.
        /// </summary>
        /// <param name="src">Source IQueryable</param>
        /// <param name="members">Names of the properties to keep from initial object type</param>
        /// <param name="aggregateOperation">Pair of property name and agregate formula (as defined for System.Linq.Dynamic library)</param>
        /// <returns>Projected IQueryable with item types matching the dynamically constructed type as defined by passed parameters</returns>
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

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Wokhan.Data.Extensions
{
    /// <summary>
    /// Extension methods for the DataRow type
    /// </summary>
    public static class DataRowExtensions
    {
        /// <summary>
        /// Returns parent rows for a given <see cref="DataRow"/>
        /// </summary>
        /// <param name="drow">Source row to retrieve parents for</param>
        /// <returns>All parents pointing to the specified row</returns>
        public static IEnumerable<DataRow> GetParentRows(this DataRow drow)
        {
            return drow.Table.ParentRelations.Cast<DataRelation>().SelectMany(r => drow.GetParentRows(r));
        }

    }
}
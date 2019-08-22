using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Wokhan.Data.Extensions
{
    /// <summary>
    /// Summary description for DataSetExtensions
    /// </summary>
    public static class DataRowExtensions
    {
        public static IEnumerable<DataRow> GetParentRows(this DataRow drow)
        {
            return drow.Table.ParentRelations.Cast<DataRelation>().SelectMany(r => drow.GetParentRows(r));
        }

    }
}
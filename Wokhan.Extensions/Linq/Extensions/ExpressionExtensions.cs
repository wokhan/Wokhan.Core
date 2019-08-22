using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Wokhan.Linq.Extensions
{
    public static class ExpressionExtensions
    {
        public static IList<MemberInfo> GetMembers<T, TR>(this Expression<Func<T, TR>> columnSelectorExpr)
        {
            return (columnSelectorExpr.Body is NewExpression ? ((NewExpression)columnSelectorExpr.Body).Members.ToArray() : new[] { ((MemberExpression)columnSelectorExpr.Body).Member });
        }

        public static Func<TR, object[]> GetValues<T, TR>(this Expression<Func<T, TR>> columnSelectorExpr)
        {
            if (columnSelectorExpr.Body is MemberExpression)
            {
                return x => new object[] { x };
            }
            else
            {
                var members = columnSelectorExpr.GetMembers();
                return x => members.Cast<PropertyInfo>().Select(m => m.GetValue(x)).ToArray();
            }
        }


    }
}

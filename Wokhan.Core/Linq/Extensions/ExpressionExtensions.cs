using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Wokhan.Linq.Extensions
{
    public static class ExpressionExtensions
    {
        public static IList<MemberInfo> GetMembers<T, TR>(this Expression<Func<T, TR>> expression)
        {
            Contract.Requires(expression != null);

            return (expression.Body is NewExpression ? ((NewExpression)expression.Body).Members.ToArray() : new[] { ((MemberExpression)expression.Body).Member });
        }

        public static Func<TR, object[]> GetValues<T, TR>(this Expression<Func<T, TR>> expression)
        {
            Contract.Requires(expression != null);
            
            if (expression.Body is MemberExpression)
            {
                return x => new object[] { x };
            }
            else
            {
                var members = expression.GetMembers();
                return x => members.Cast<PropertyInfo>().Select(m => m.GetValue(x)).ToArray();
            }
        }


    }
}

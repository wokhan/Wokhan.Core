using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Wokhan.Linq.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="System.Linq.Expressions.Expression"/> class
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Get all members used in a LINQ Expression (built from a lambda for instance)
        /// </summary>
        /// <typeparam name="T">Expression's input type</typeparam>
        /// <typeparam name="TR">Expression's return type</typeparam>
        /// <param name="expression">Source expression</param>
        /// <returns>A list of <see cref="MemberInfo"/></returns>
        public static IList<MemberInfo> GetMembers<T, TR>(this Expression<Func<T, TR>> expression)
        {
            Contract.Requires(expression != null);

            return (expression.Body is NewExpression ? ((NewExpression)expression.Body).Members.ToArray() : new[] { ((MemberExpression)expression.Body).Member });
        }

        /// <summary>
        /// Returns a getter to access all properties values from an expression, as an objects array.
        /// <code>
        /// TODO: add examples
        /// </code>
        /// </summary>
        /// <typeparam name="T">Expression input type</typeparam>
        /// <typeparam name="TR">Expression output type</typeparam>
        /// <param name="expression">Source expression</param>
        /// <returns>A <see cref="Func{TR, Object[]}"/> used to access all property values (as an object array)</returns>
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

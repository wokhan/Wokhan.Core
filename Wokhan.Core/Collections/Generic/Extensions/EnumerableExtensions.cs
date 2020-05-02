using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Wokhan.Linq.Extensions;
using Wokhan.Core.Comparers;
using Wokhan.Core.Linq;
using System.Diagnostics.Contracts;

namespace Wokhan.Collections.Generic.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ApplyToAll<T>(this IEnumerable<T> src, Action<T> action)
        {
            Contract.Requires(src != null);
            Contract.Requires(action != null);
            
            return src.Select(_ => { action(_); return _; });
        }

        public static double AverageChecked<T>(this IEnumerable<T> src, bool ignoreErrors = false) where T : IConvertible
        {
            Contract.Requires(src != null);

            var converter = new DoubleConverter();
            var s = src;
            if (ignoreErrors)
            {
                s = s.Where(c => converter.IsValid(c));
            }
            return src.Average(x => (double)converter.ConvertFrom(x));
        }

        public static int GreatestCommonDiv(this IEnumerable<int> src)
        {
            Contract.Requires(src != null);

            return src.OrderBy(a => a).Aggregate((a, b) => GreatestCommonDiv(a, b));
        }

        private static int GreatestCommonDiv(int a, int b)
        {
            int rem;

            while (b != 0)
            {
                rem = a % b;
                a = b;
                b = rem;
            }

            return a;
        }


        public static IQueryable<TResult> Select<TResult>(this IQueryable src, IEnumerable<string> selectors)
        {
            Contract.Requires(src != null);

            if (typeof(TResult) == typeof(object))
            {
                //var config = new ParsingConfig() { UseDynamicObjectClassForAnonymousTypes = true };
                return src.Select($"new({string.Join(",", selectors)})").Cast<TResult>();
            }
            return src.Select<TResult>($"new({string.Join(",", selectors)})");
        }


        public static IEnumerable<object[]> AsObjectCollection(this IEnumerable src, params string[] attributes)
        {
            return AsObjectCollection(src.Cast<object>(), attributes);
        }

        public static IEnumerable<object[]> AsObjectCollection<T>(this IEnumerable<T> src, params string[] attributes)
        {
            Contract.Requires(src != null);

            var innertype = src.GetInnerType();
            if (innertype.IsArray)
            {
                return ((IEnumerable<object[]>)src);
            }
            else
            {

                if (attributes == null)
                {
                    attributes = innertype.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(a => a.Name).ToArray();
                }

                var param = Expression.Parameter(typeof(object));
                var expa = Expression.Parameter(typeof(Exception));
                var ide_cstr = typeof(InvalidDataException).GetConstructor(new[] { typeof(string), typeof(Exception) });

                var casted = Expression.Convert(param, innertype);

                Func<string, Expression> propertyGet = a => Expression.Property(casted, a);
                // Assuming dynamic...
                /*if (innertype == typeof(object))
                {

                    propertyGet = a =>
                    {
                        var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, a, innertype, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                        return Expression.Dynamic(binder, innertype, casted);
                    };
                }
                else
                {
                    propertyGet = a => Expression.Property(casted, a);
                }*/
                var atrs = attributes.Select(a =>
                    Expression.TryCatch(
                        Expression.Block(
                            Expression.Convert(propertyGet(a), typeof(object))
                        ),
                    Expression.Catch(expa,
                        Expression.Block(
                            Expression.Throw(Expression.New(ide_cstr, Expression.Constant(a), expa)),
                            Expression.Constant(null))))
                ).ToList();

                var attrExpr = Expression.Lambda<Func<T, object[]>>(Expression.NewArrayInit(typeof(object), atrs), param).Compile();

                return src.Select(x => { if (x == null) { throw new Exception("Should never get there."); } return attrExpr(x); });
            }
        }

        //public static IEnumerable<object[]> AsObjectCollection(this IEnumerable src, params string[] attributes)
        //{
        //    var innertype = src.GetInnerType();
        //    if (innertype.IsArray)
        //    {
        //        return ((IEnumerable<object[]>)src);
        //    }
        //    else
        //    {
        //        var param = Expression.Parameter(typeof(object));
        //        var attrExpr = Expression.Lambda<Func<T, object[]>>(Expression.NewArrayInit(typeof(object), attributes.Select(a => Expression.Convert(Expression.Property(Expression.Convert(param, innertype), a), typeof(object)))), param).Compile();

        //        return ((IEnumerable<dynamic>)src).Select(attrExpr);
        //    }
        //}

        public static Type GetInnerType<T>(this IEnumerable<T> src)
        {
            Contract.Requires(src != null);

            return src.GetType().GenericTypeArguments.FirstOrDefault();
        }

        public static Type GetInnerType(this IEnumerable src)
        {
            Contract.Requires(src != null);

            return src.GetType().GenericTypeArguments.FirstOrDefault();
        }

        public static IOrderedEnumerable<T[]> OrderByMany<T>(this IEnumerable<T[]> src, int[] indexes)
        {
            Contract.Requires(src != null);
            Contract.Requires(indexes != null);

            IOrderedEnumerable<T[]> ret = src.OrderBy(a => a.Length > 0 ? a[indexes[0]] : default(T));
            for (int i = 1; i < indexes.Length; i++)
            {
                var ic = i;
                ret = ret.ThenBy(a => a.Length > ic ? a[ic] : default(T));
            }

            return ret;
        }

        public static IOrderedQueryable<T> OrderByMany<T>(this IQueryable<T> src, IEnumerable<string> sorters)
        {
            Contract.Requires(src != null);
            Contract.Requires(sorters != null);

            var param = ParameterExpression.Parameter(typeof(T));

            var initSorter = Expression.Lambda<Func<T, dynamic>>(Expression.Property(param, sorters.First().Trim('-')), param);
            var ret = sorters.First().EndsWith("-") ? src.OrderByDescending(initSorter) : src.OrderBy(initSorter);
            foreach (var attr in sorters.Skip(1))
            {
                var sorter = Expression.Lambda<Func<T, dynamic>>(Expression.Property(param, attr.Trim('-')), param);
                ret = attr.EndsWith("-") ? ret.ThenByDescending(sorter) : ret.ThenBy(sorter);
            }

            return ret;
        }

        [Obsolete("Use DynamicQueryableExtensions from System.Linq.Dynamic package instead")]
        public static IOrderedQueryable<dynamic> OrderByMany(this IQueryable src, params string[] attributes)
        {
            Contract.Requires(src != null);

            var innertype = src.GetInnerType();
            var m = typeof(EnumerableExtensions).GetMethod(nameof(OrderByManyTyped)).MakeGenericMethod(innertype);
            return (IOrderedQueryable<dynamic>)m.Invoke(null, new object[] { src, attributes });
        }

        public static IOrderedQueryable<T> OrderByManyTyped<T>(IQueryable<T> src, params string[] attributes)
        {
            Contract.Requires(src != null);

            var param = Expression.Parameter(typeof(T));

            var ret = src.OrderBy(Expression.Lambda<Func<T, dynamic>>(Expression.Property(param, attributes.First()), param));
            foreach (var attr in attributes.Skip(1))
            {
                ret = ret.ThenBy(Expression.Lambda<Func<T, dynamic>>(Expression.Property(param, attr), param));
            }

            return ret;
        }

        public static IOrderedEnumerable<T[]> OrderByMany<T>(this IEnumerable<T[]> src, int columnsToTake, int columnsToSkip = 0)
        {
            Contract.Requires(src != null);

            IOrderedEnumerable<T[]> ret = src.OrderBy(a => a.Length > columnsToSkip ? a[columnsToSkip] : default(T));
            for (int i = columnsToSkip + 1; i < columnsToSkip + columnsToTake; i++)
            {
                var ic = i;
                ret = ret.ThenBy(a => a.Length > ic ? a[ic] : default(T));
            }

            return ret;
        }

        [Obsolete("Use DynamicQueryableExtensions from System.Linq.Dynamic package instead")]
        public static IOrderedQueryable<T[]> OrderByMany<T>(this IQueryable<T[]> src, int columnsToTake, int columnsToSkip = 0)
        {
            Contract.Requires(src != null);

            IOrderedQueryable<T[]> ret = src.OrderBy(a => a.Length > columnsToSkip ? a[columnsToSkip] : default(T));
            for (int i = columnsToSkip + 1; i < columnsToSkip + columnsToTake; i++)
            {
                var ic = i;
                ret = ret.ThenBy(a => a.Length > ic ? a[ic] : default(T));
            }

            return ret;
        }
        public static IOrderedEnumerable<T[]> OrderByAll<T>(this IEnumerable<IEnumerable<T>> src, int skip = 0)
        {
            Contract.Requires(src != null);

            return src.Select(o => o.ToArray()).OrderByAll(skip);
        }

        public static IOrderedEnumerable<T> OrderByAll<T>(this IEnumerable<T> src, int skip = 0)
        {
            Contract.Requires(src != null);

            var allmembers = typeof(T).GetFields().Where(m => typeof(IComparable).IsAssignableFrom(m.FieldType)).ToArray();
            IOrderedEnumerable<T> ret = src.OrderBy(m => allmembers[skip].GetValue(m));
            for (int i = 1 + skip; i < allmembers.Length - 1; i++)
            {
                var ic = i;
                ret = ret.ThenBy(a => allmembers[ic].GetValue(a));
            }

            return ret;
        }

        public static IOrderedQueryable<dynamic> OrderByAll(this IQueryable src)
        {
            Contract.Requires(src != null);

            var innertype = src.GetInnerType();
            var m = typeof(EnumerableExtensions).GetMethod(nameof(OrderByAllTyped)).MakeGenericMethod(innertype);
            return (IOrderedQueryable<dynamic>)m.Invoke(null, new object[] { src, 0 });
        }

        public static IOrderedQueryable<T> OrderByAllTyped<T>(this IQueryable<T> src, int skip = 0)
        {
            Contract.Requires(src != null);

            var allmembers = typeof(T).GetProperties().Where(m => m.PropertyType.IsGenericType || typeof(IComparable).IsAssignableFrom(m.PropertyType)).ToArray();

            IOrderedQueryable<T> ret = src.OrderBy(m => allmembers[skip].GetValue(m));
            for (int i = skip; i < allmembers.Length; i++)
            {
                var ic = i;
                ret = ret.ThenBy(a => allmembers[ic].GetValue(a));
            }

            return ret;
        }

        public static void ReplaceAll<T>(this ObservableCollection<T> src, IEnumerable<T> all)
        {
            Contract.Requires(src != null);

            src.Clear();
            
            src.AddAll(all);
        }


        public static ParallelQuery<T> AsParallel<T>(this IEnumerable<T> src, bool useParallelism)
        {
            Contract.Requires(src != null);

            var ret = src.AsParallel();
            return (useParallelism ? ret : ret.WithDegreeOfParallelism(1));
        }

        public static void AddAll<T>(this ICollection<T> src, IEnumerable<T> all)
        {
            Contract.Requires(src != null);
            Contract.Requires(all != null);

            foreach (var x in all)
            {
                src.Add(x);
            }
        }

       


        public static object ToObject(this IList src, Type targetclass, string[] attributes)
        {
            Contract.Requires(src != null);
            Contract.Requires(targetclass != null);
            Contract.Requires(attributes != null);

            var trg = Activator.CreateInstance(targetclass);

            var pr = attributes.Join(targetclass.GetProperties(), a => a, b => b.Name, (a, b) => b).ToList();
            if (src.Count < pr.Count)
            {
                throw new ArgumentOutOfRangeException("Source has fewer values than expected");
            }

            for (int i = 0; i < pr.Count; i++)
            {
                var value = src[i];
                if (value != null && !DBNull.Value.Equals(value))
                {
                    pr[i].SetValue(trg, value);
                }
            }

            return trg;
        }

        public static T ToObject<T>(this IList src, string[] attributes)
        {
            Contract.Requires(src != null);

            return (T)ToObject(src, typeof(T), attributes);
        }

        public static T ToObject<T>(this IEnumerable src, string[] attributes)
        {
            Contract.Requires(src != null);

            return (T)ToObject(src.Cast<object>().ToArray(), typeof(T), attributes);
        }

        /*public static T ToObject<T>(this string[] o, string[] attributes) where T : new()
        {
            var trg = (T)Activator.CreateInstance(typeof(T));

            var pr = attributes.Join(typeof(T).GetProperties(), a => a, b => b.Name, (a, b) => b).ToList();
            for (int i = 0; i < pr.Count; i++)
            {
                pr[i].SetValue(trg, o[i]);
            }

            return trg;
        }*/

        static Action<string, double, double> defaultCallback = (message, value, max) => Console.WriteLine($"{value}/{max} - {message}");

        public static IEnumerable<T> WithProgress<T>(this IEnumerable<T> src, Func<T, string> captionGetter, Action<string, double, double> callback = null, double? max = null)
        {
            Contract.Requires(src != null);

            double p0max = max ?? src.Count();
            var p0cnt = 0;
            var cb = (callback ?? defaultCallback);
            return src.ApplyToAll(x =>
            {
                Interlocked.Increment(ref p0cnt);
                cb(captionGetter(x), p0cnt, p0max);
            });
        }

        public static IEnumerable<T> Merge<T>(this IEnumerable<T> src, IEnumerable<T> added, IEqualityComparer<T> comparer)
        {
            Contract.Requires(src != null);
            Contract.Requires(added != null);
            Contract.Requires(comparer != null);

            var targetSet = new Set<T>(comparer);
            targetSet.UnionWith(added);
            return src.Where(x => !targetSet.Contains(x, comparer)).Concat(added);
        }

        public static IEnumerable<T> Merge<T>(this IEnumerable<T> source, IEnumerable<T> added, Func<T, object> predicate)
        {
            Contract.Requires(source != null);
            Contract.Requires(added != null);

            return Merge(source, added, new GenericComparer<T>(predicate));
        }

        public static IEnumerable<T> WithProgress<T>(this IEnumerable<T> src, Action<double> callback)
        {
            Contract.Requires(src != null);

            var p0cnt = 0;
            return src.ApplyToAll(x =>
            {
                Interlocked.Increment(ref p0cnt);
                callback(p0cnt);
            });
        }

        
        public static DataTable AsDataTable<T>(this IEnumerable<T> src, IList<string> headers = null, string name = "Default")
        {
            Contract.Requires(src != null);

            var ret = new DataTable(name);
            ret.BeginLoadData();

            var members = typeof(T).GetProperties();
            DataColumn[] cols = null;

            if (headers != null)
            {
                cols = headers.Select(m => new DataColumn(m)).ToArray();
            }

            Func<T, object[]> arraygetter;
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                if (cols == null)
                {
                    var firstItem = (IEnumerable<object>)src.FirstOrDefault(); ;
                    cols = Enumerable.Range(0, firstItem?.Count() ?? 0).Select(m => new DataColumn("P" + m, typeof(object))).ToArray();
                }
                arraygetter = x => ((IEnumerable)x).Cast<object>().ToArray();
            }
            else
            {
                cols = cols ?? members.Select(m => new DataColumn(m.Name, m.PropertyType)).ToArray();
                arraygetter = x => members.Select(m => m.GetValue(x)).ToArray();
            }

            ret.Columns.AddRange(cols);


            foreach (T o in src)
            {
                ret.Rows.Add(arraygetter(o));
            }

            ret.AcceptChanges();
            ret.EndLoadData();

            return ret;
        }


        public static DataTable AsDataTable(this IEnumerable<object[]> collection, string name, DataColumn[] cols)
        {
            Contract.Requires(collection != null);

            var ret = new DataTable(name);
            ret.Columns.AddRange(cols);

            ret.BeginLoadData();

            foreach (var o in collection)
            {
                ret.Rows.Add(o);
            }

            ret.EndLoadData();
            //ret.AcceptChanges();

            return ret;
        }

        /*public static DataTable AsDataTable<T, TValues>(this IEnumerable<T> collection, Expression<Func<T, TValues>> GetValuesDelegate, string name = "Default")
        {
            DataTable ret = new DataTable(name);
            ret.BeginLoadData();
            var members = ((NewExpression)GetValuesDelegate.Body).Members;
            ret.Columns.AddRange(members.Cast<PropertyInfo>().Select(m => new DataColumn(m.Name, m.PropertyType)).ToArray());

            var getter = GetValuesDelegate.Compile();
            Func<TValues, object[]> arraygetter = x => members.Cast<PropertyInfo>().Select(m => m.GetValue(x)).ToArray();
            foreach (T o in collection)
            {
                ret.Rows.Add(arraygetter(getter(o)));
            }

            ret.AcceptChanges();
            ret.EndLoadData();
            return ret;
        }*/

        /*public static DataTable AsDataTable<T>(this IEnumerable<T> collection, string name, Dictionary<string, Type> types, Func<T, object[]> GetValuesDelegate)
        {
            DataTable ret = new DataTable(name);
            ret.BeginLoadData();
            ret.Columns.AddRange(types.Select(m => new DataColumn(m.Key, m.Value)).ToArray());

            foreach (T o in collection)
            {
                ret.Rows.Add(GetValuesDelegate(o));
            }

            ret.AcceptChanges();
            ret.EndLoadData();
            return ret;
        }*/

        public static DataTable ToPivotTable<T, TColumn, TRow, TData>(this IEnumerable<T> src, Expression<Func<T, TRow>> keysSelector, Expression<Func<T, TColumn>> pivotSelectorExpr, Func<IEnumerable<T>, TData> aggregateSelector, string tableName = "Default")
        {
            Func<T, TColumn> columnSelector;
            Func<TColumn, object[]> arraygetter;

            var pivoted = PivotDataInternal(src, keysSelector, pivotSelectorExpr, aggregateSelector, out arraygetter, out columnSelector);

            var memberKeys = keysSelector.GetMembers();
            var columns = memberKeys.Select(m => m.Name).Concat(src.SelectMany(c => arraygetter(columnSelector(c))).Distinct().Select(c => c.ToString())).ToList();

            return pivoted.ToList().AsDataTable(columns, tableName);
        }


        public static IEnumerable<dynamic> Pivot<T, TKeys, TPivoted, TAggregate>(this IEnumerable<T> src, Expression<Func<T, TKeys>> keysSelector, Expression<Func<T, TPivoted>> pivotSelectorExpr, Func<IEnumerable<T>, TAggregate> aggregateSelector, string tableName = "Default")
        {
            Func<TPivoted, object[]> arraygetter;
            Func<T, TPivoted> columnSelector;

            var pivoted = PivotDataInternal(src, keysSelector, pivotSelectorExpr, aggregateSelector, out arraygetter, out columnSelector);

            var memberKeys = keysSelector.GetMembers().Cast<PropertyInfo>().ToList();
            var properties = memberKeys.Select(m => new DynamicProperty(m.Name, m.PropertyType))
                                       .Concat(src.SelectMany(c => arraygetter(columnSelector(c)))
                                       .Distinct()
                                       .Select(c => new DynamicProperty(c.ToString(), aggregateSelector.Method.ReturnType)))
                                       .ToArray();
            //var properties = memberKeys.Concat(membersCols).Select(m => new DynamicProperty(m.Name, m.PropertyType));

            var dynobj = DynamicClassFactory.CreateType(properties);

            return pivoted.Select(args => Activator.CreateInstance(dynobj, args));
        }

        private static IEnumerable<object[]> PivotDataInternal<T, TKeys, TPivoted, TAggregate>(IEnumerable<T> src, Expression<Func<T, TKeys>> keysSelector, Expression<Func<T, TPivoted>> pivotSelectorExpr, Func<IEnumerable<T>, TAggregate> aggregateSelector, out Func<TPivoted, object[]> arraygetter, out Func<T, TPivoted> columnSelector)
        {
            Contract.Requires(src != null);
            Contract.Requires(keysSelector != null);
            Contract.Requires(pivotSelectorExpr != null);

            arraygetter = pivotSelectorExpr.GetValues();

            var columnSelectorLocal = pivotSelectorExpr.Compile();
            var pivotValues = src.Select(columnSelectorLocal).Distinct().ToList();
            var arraygetterRow = keysSelector.GetValues();

            columnSelector = columnSelectorLocal;

            return src.GroupBy(keysSelector.Compile())
                            .Select(rowGroup => arraygetterRow(rowGroup.Key).Concat(pivotValues.GroupJoin(
                                    rowGroup,
                                    c => c,
                                    r => columnSelectorLocal(r),
                                    (c, columnGroup) => aggregateSelector(columnGroup)).Cast<object>()).ToArray());
        }
    }
}

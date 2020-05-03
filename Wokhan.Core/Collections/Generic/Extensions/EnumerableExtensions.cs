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
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Applies an <see cref="Action{T}"/> to all items in an enumeration, when enumerated.
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source enumeration</param>
        /// <param name="action">The action to perform on each items</param>
        /// <returns>Original enumeration with items modified or used by the specified action</returns>
        public static IEnumerable<T> ApplyToAll<T>(this IEnumerable<T> src, Action<T> action)
        {
            Contract.Requires(src != null);
            Contract.Requires(action != null);

            return src.Select(_ => { action(_); return _; });
        }

        /// <summary>
        /// Computes an average for all values in an enumerable, ensuring they can be converted to double (using a <see cref="DoubleConverter"/>).
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source enumeration</param>
        /// <param name="ignoreErrors">Specify whether errors are blocking</param>
        /// <returns>The computed average</returns>
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

        /// <summary>
        /// Computes the greatest common divisor for an integer enumeration
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <returns>The greatest common divisor for all items</returns>
        public static int GreatestCommonDiv(this IEnumerable<int> src)
        {
            Contract.Requires(src != null);

            // Why ordered?
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

        /// <summary>
        /// Turns a generic <see cref="IEnumerable"/> into an object[] enumeration (each property being mapped into the array)
        /// </summary>
        /// <param name="src">Source enumeration</param>
        /// <param name="properties">Name of the properties to use to populate the array</param>
        /// <returns></returns>
        public static IEnumerable<object[]> AsObjectCollection(this IEnumerable src, params string[] properties)
        {
            return AsObjectCollection(src.Cast<object>(), properties);
        }

        /// <summary>
        /// Turns a generic <see cref="IEnumerable{T}"/> into an object[] enumeration (each property being mapped into the array)
        /// </summary>
        /// <param name="src">Source enumeration</param>
        /// <param name="properties">Name of the properties to use to populate the array</param>
        /// <returns></returns>
        public static IEnumerable<object[]> AsObjectCollection<T>(this IEnumerable<T> src, params string[] properties)
        {
            Contract.Requires(src != null);

            var innertype = src.GetInnerType();
            if (innertype.IsArray)
            {
                return ((IEnumerable<object[]>)src);
            }
            else
            {

                if (properties == null)
                {
                    properties = innertype.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(a => a.Name).ToArray();
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
                var atrs = properties.Select(a =>
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

        /// <summary>
        /// Returns the actual "inner" type of an element in a collection when T is also a generic type
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source enumeration</param>
        /// <returns>The "inner" type for items of generic types</returns>
        public static Type GetInnerType<T>(this IEnumerable<T> src)
        {
            Contract.Requires(src != null);

            return src.GetType().GenericTypeArguments.FirstOrDefault();
        }

        /// <summary>
        /// Returns the actual "inner" type of an element in a <see cref="IEnumerable"/> containing genericly typed items
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source enumeration</param>
        /// <returns>The "inner" type for items of generic types</returns>
        public static Type GetInnerType(this IEnumerable src)
        {
            Contract.Requires(src != null);

            return src.GetType().GenericTypeArguments.FirstOrDefault();
        }

        /// <summary>
        /// Orders an enumerable of T[] by multiple items (using indexes)
        /// </summary>
        /// <typeparam name="T">Items type (passed as arrays)</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="indexes">Indexes to get elements at for ordering</param>
        /// <returns>An ordered enumerable</returns>
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

        /// <summary>
        /// Orders an <see cref="IQueryable{T}"/> by the specified properties
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="sorters">List of properties to order by (if ends with a "-"</param>
        /// <returns></returns>
        [Obsolete("Use DynamicQueryableExtensions instead (which leverages System.Linq.Dynamic)")]
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

        [Obsolete("Use DynamicQueryableExtensions instead (which leverages System.Linq.Dynamic)")]
        public static IOrderedQueryable<dynamic> OrderByMany(this IQueryable src, params string[] attributes)
        {
            Contract.Requires(src != null);

            var innertype = src.GetInnerType();
            var m = typeof(EnumerableExtensions).GetMethod(nameof(OrderByManyTyped)).MakeGenericMethod(innertype);
            return (IOrderedQueryable<dynamic>)m.Invoke(null, new object[] { src, attributes });
        }

        [Obsolete("Use DynamicQueryableExtensions instead (which leverages System.Linq.Dynamic)")]
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

        /// <summary>
        /// Orders an enumerable of T[] by the first "<paramref name="take"/>" values of each array (optionnally skipping some)
        /// </summary>
        /// <typeparam name="T">Enumerable items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="take">Number of values to take in the array for each item</param>
        /// <param name="skip">Number of values to skip in the array for each item</param>
        /// <returns></returns>
        public static IOrderedEnumerable<T[]> OrderByMany<T>(this IEnumerable<T[]> src, int take, int skip = 0)
        {
            Contract.Requires(src != null);

            IOrderedEnumerable<T[]> ret = src.OrderBy(a => a.Length > skip ? a[skip] : default(T));
            for (int i = skip + 1; i < skip + take; i++)
            {
                var ic = i;
                ret = ret.ThenBy(a => a.Length > ic ? a[ic] : default(T));
            }

            return ret;
        }

        /// <summary>
        /// Orders a queryable of T[] by the first "<paramref name="take"/>" values of each array (optionnally skipping some)
        /// </summary>
        /// <typeparam name="T">Enumerable items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="take">Number of values to take in the array for each item</param>
        /// <param name="skip">Number of values to skip in the array for each item</param>
        /// <returns></returns>
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

        /// <summary>
        /// Orders an IEnumerable which items are also IEnumerable by all it's inner enumeration values, optionnally skipping some
        /// </summary>
        /// <typeparam name="T">Inner enumerable items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="skip">Number of items to skip in the inner enumerable used for sorting</param>
        /// <returns></returns>
        public static IOrderedEnumerable<T[]> OrderByAll<T>(this IEnumerable<IEnumerable<T>> src, int skip = 0)
        {
            Contract.Requires(src != null);

            return src.Select(o => o.ToArray()).OrderByAll(skip);
        }

        /// <summary>
        /// Orders an <see cref="IEnumerable{T}"/> using all fields of the T type
        /// </summary>
        /// <typeparam name="T">Inner enumerable items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="skip">Number of fields to ignore when sorting</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sorts an untyped IQueryable by guessing the inner type, using <see cref="OrderByAllTyped{T}(IQueryable{T}, int)" />
        /// <seealso cref="OrderByAllTyped{T}(IQueryable{T}, int)"/>
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <returns></returns>
        public static IOrderedQueryable<dynamic> OrderByAll(this IQueryable src)
        {
            Contract.Requires(src != null);

            var innertype = src.GetInnerType();
            var m = typeof(EnumerableExtensions).GetMethod(nameof(OrderByAllTyped)).MakeGenericMethod(innertype);
            return (IOrderedQueryable<dynamic>)m.Invoke(null, new object[] { src, 0 });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Replaces all items it an <see cref="ICollection{T}"/> source by the specified items.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="newItems">Items to add</param>
        public static void ReplaceAll<T>(this ICollection<T> src, IEnumerable<T> newItems)
        {
            Contract.Requires(src != null);

            src.Clear();

            src.AddAll(newItems);
        }

        /// <summary>
        /// Shortcut to create a ParallelQuery, optionnally using only one thread (disabling in fact parallelism but keeping the same return type).
        /// Used in fluent queries.
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="useParallelism">Indicates whether to actually use parallelism</param>
        /// <returns>A parallel query with either 1 degree of parallelism (if disabled) or default one (if enabled)</returns>
        public static ParallelQuery<T> AsParallel<T>(this IEnumerable<T> src, bool useParallelism)
        {
            Contract.Requires(src != null);

            var ret = src.AsParallel();
            return (useParallelism ? ret : ret.WithDegreeOfParallelism(1));
        }

        /// <summary>
        /// Add all specified items to a collection using a simple loop
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="newItems">Items to add</param>
        public static void AddAll<T>(this ICollection<T> src, IEnumerable<T> newItems)
        {
            Contract.Requires(src != null);
            Contract.Requires(newItems != null);

            foreach (var x in newItems)
            {
                src.Add(x);
            }
        }


        /// <summary>
        /// Turns a generic list of values to an object, mapping each list items to properties of the specified target type.
        /// <seealso cref="ToObject{T}(IList, string[])"/>
        /// <seealso cref="ToObject{T}(IEnumerable, string[])"/>
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <param name="targetclass">Type of the object to create</param>
        /// <param name="attributes">Attributes to map</param>
        /// <returns></returns>
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

        /// <summary>
        /// Turns a generic list of values to an object, mapping each list items to properties of the specified target type.
        /// Uses <see cref="ToObject(IList, Type, string[])"/>
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <param name="attributes">Attributes to map</param>
        /// <returns></returns>
        public static T ToObject<T>(this IList src, string[] attributes)
        {
            Contract.Requires(src != null);

            return (T)ToObject(src, typeof(T), attributes);
        }

        /// <summary>
        /// Turns a generic list of values to an object, mapping each list items to properties of the specified target type.
        /// Uses <see cref="ToObject(IList, Type, string[])"/>
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <param name="attributes">Attributes to map</param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds a progress callback to any enumeration, which will increment an internal counter when enumerating and call the specified callback (or write to console if none specified).
        /// Note: if <paramref name="max"/> is not specified, Count() will be called on the source collection, which would result in an enumeration and can have side effects.
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="captionGetter">Method used to build the caption sent to the callback</param>
        /// <param name="callback">Callback (defaults to Console.WriteLine)</param>
        /// <param name="max">Number of total items in the source collection</param>
        /// <returns></returns>
        [Obsolete("Should use WithProgress<T>(this IEnumerable<T> src, Action<double> callback) instead")]
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

        /// <summary>
        /// Adds a progress callback to any enumeration, which will increment an internal counter when enumerating and call the specified callback.
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="callback">Callback taking the current increment value as parameter</param>
        /// <returns></returns>
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



        /// <summary>
        /// Merges two enumerables using the specified comparer.
        /// Does use a Set (as defined in .Net reference source) internally to optimize merging.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="added">Collection to merge the source with</param>
        /// <param name="comparer">Equality comparer</param>
        /// <returns>Merged enumeration</returns>
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> src, IEnumerable<T> added, IEqualityComparer<T> comparer)
        {
            Contract.Requires(src != null);
            Contract.Requires(added != null);
            Contract.Requires(comparer != null);

            var targetSet = new Set<T>(comparer);
            targetSet.UnionWith(added);
            return src.Where(x => !targetSet.Contains(x, comparer)).Concat(added);
        }


        /// <summary>
        /// Merges two enumerables using the specified comparison predicate (the predicate returns the property or value to compare on, using <see cref="GenericComparer{T}"/>).
        /// <seealso cref="Merge{T}(IEnumerable{T}, IEnumerable{T}, IEqualityComparer{T})"/>
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="added">Collection to merge the source with</param>
        /// <param name="predicate">Predicate to obtain the property to compare on</param>
        /// <returns>Merged enumeration</returns>
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> src, IEnumerable<T> added, Func<T, object> predicate)
        {
            Contract.Requires(src != null);
            Contract.Requires(added != null);

            return Merge(src, added, new GenericComparer<T>(predicate));
        }


        /// <summary>
        /// Creates a <see cref="DataTable"/> from an <see cref="IEnumerable{T}"/>, optionnally overriding the column headers.
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="headers">Headers to use for the DataTable</param>
        /// <param name="name">DataTable name (optional)</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> src, IList<string> headers = null, string name = "Default")
        {
            Contract.Requires(src != null);


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

            return src.Select(arraygetter).ToDataTable(cols, name);
        }

        /// <summary>
        /// Creates a <see cref="DataTable"/> from an IEnumerable of object[] with the specified columns.
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <param name="cols">Headers to use for the DataTable</param>
        /// <param name="name">DataTable name (optional)</param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IEnumerable<object[]> src, DataColumn[] cols, string name = "Default")
        {
            Contract.Requires(src != null);

            var ret = new DataTable(name);
            ret.Columns.AddRange(cols);

            ret.BeginLoadData();

            foreach (var o in src)
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

        /// <summary>
        /// Creates a pivot table (a table where columns are created from specified values in a collection) from a IEnumerable of <typeparamref name="T"/>, using the specified selector and aggregators.
        /// <code>
        /// Example to be added soon
        /// </code>
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <typeparam name="TPivoted">Type of the pivoted data</typeparam>
        /// <typeparam name="TKeys">Type of the keys</typeparam>
        /// <typeparam name="TAggregate">Type of computed aggregated data for each pivot</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="keysSelector">Keys selector (used as keys for the groups aggregation for pivoted values will be computed on)</param>
        /// <param name="pivotSelectorExpr">Expression to get the properties to compute the pivot on</param>
        /// <param name="aggregateSelector">Aggregation calculation method</param>
        /// <param name="tableName">Name of the created table</param>
        /// <returns></returns>
        public static DataTable ToPivotTable<T, TPivoted, TKeys, TAggregate>(this IEnumerable<T> src, Expression<Func<T, TKeys>> keysSelector, Expression<Func<T, TPivoted>> pivotSelectorExpr, Func<IEnumerable<T>, TAggregate> aggregateSelector, string tableName = "Default")
        {
            Func<T, TPivoted> columnSelector;
            Func<TPivoted, object[]> arraygetter;

            var pivoted = PivotDataInternal(src, keysSelector, pivotSelectorExpr, aggregateSelector, out arraygetter, out columnSelector);

            var memberKeys = keysSelector.GetMembers();
            var columns = memberKeys.Select(m => m.Name).Concat(src.SelectMany(c => arraygetter(columnSelector(c))).Distinct().Select(c => c.ToString())).ToList();

            return pivoted.ToList().ToDataTable(columns, tableName);
        }


        /// <summary>
        /// Creates a pivot table (a table where columns are created from specified values in a collection) from a IEnumerable of <typeparamref name="T"/>, using the specified selector and aggregators.
        /// <code>
        /// Example to be added soon
        /// </code>
        /// </summary>
        /// <typeparam name="T">Items type</typeparam>
        /// <typeparam name="TPivoted">Type of the pivoted data</typeparam>
        /// <typeparam name="TKeys">Type of the keys</typeparam>
        /// <typeparam name="TAggregate">Type of computed aggregated data for each pivot</typeparam>
        /// <param name="src">Source collection</param>
        /// <param name="keysSelector">Keys selector (used as keys for the groups aggregation for pivoted values will be computed on)</param>
        /// <param name="pivotSelectorExpr">Expression to get the properties to compute the pivot on</param>
        /// <param name="aggregateSelector">Aggregation calculation method</param>
        /// <returns></returns>
        public static IEnumerable<dynamic> Pivot<T, TKeys, TPivoted, TAggregate>(this IEnumerable<T> src, Expression<Func<T, TKeys>> keysSelector, Expression<Func<T, TPivoted>> pivotSelectorExpr, Func<IEnumerable<T>, TAggregate> aggregateSelector)
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

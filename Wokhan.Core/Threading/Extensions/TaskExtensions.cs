using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wokhan.Threading.Extensions
{
    /// <summary>
    /// Extensions methods for the <see cref="Task"/> class.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Wait for all specified tasks to and and returns all results as an enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static IEnumerable<T> WaitAllAndReturn<T>(this IEnumerable<Task<T>> src)
        {
            var globalTask = Task.WhenAll(src);
            globalTask.Wait();

            return globalTask.Result?.AsEnumerable();

        }

        /// <summary>
        /// Allows to handle exceptions in a LINQ async enumeration (invoking the given <paramref name="action"/> if any when failing)
        /// </summary>
        /// <typeparam name="T">Items enumerable</typeparam>
        /// <param name="src">Collection of tasks</param>
        /// <param name="action">Callback called when an exception is catched while enumerating</param>
        /// <returns>The same enumeration, with exception handling added</returns>
        public static IEnumerable<Task<T>> WithExceptionHandling<T>(this IEnumerable<Task<T>> src, Action<Exception> action = null) where T : class
        {
            return src.Select(async t =>
                    {
                        try
                        {
                            return await t;
                        }
                        catch (Exception ex)
                        {
                            action?.Invoke(ex);
                            return (T)null;
                        }
                    });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wokhan.Threading.Extensions
{
    public static class TaskExtensions
    {
        public static IEnumerable<T> WaitAllAndReturn<T>(this IEnumerable<Task<T>> src)
        {
            var globalTask = Task.WhenAll(src);
            globalTask.Wait();

            return globalTask.Result?.AsEnumerable();

        }
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
#nullable enable
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wokhan.Core.Extensions;

namespace Wokhan.Collections.Extensions
{
    /// <summary>
    /// Extensions methods for the <see cref="Dictionary{TKey, TValue}"/> class.
    /// </summary>
    public static class DictionaryExtensions
    {
        [Obsolete("Not really useful")]
        public static void AddIfValued(this IDictionary propertyCollection, string propertyName, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                propertyCollection[propertyName] = value;
            }
        }

        [Obsolete("Not really useful")]
        public static void AddIfValued(this IDictionary propertyCollection, string propertyName, bool value)
        {
            if (value)
            {
                propertyCollection[propertyName] = value;
            }
        }

        /// <summary>
        /// WARNING: PROTOTYPE. Do not use until this notice is removed since memory impact has not been verified yet.
        /// Tries to get a value from the source dictionary, and asynchronously adds it if needed, using the provided async value resolver.
        /// </summary>
        /// <typeparam name="TKey">Dictionary key type</typeparam>
        /// <typeparam name="TValue">Dictionary value type</typeparam>
        /// <param name="src">Source dictionary</param>
        /// <param name="key">Key for the entry to get (or set)</param>
        /// <param name="asyncValueResolver">Async method to compute the value to add</param>
        /// <returns>The retrieved value</returns>
        public static async Task<TValue> GetOrSetValueAsync<TKey, TValue>(this IDictionary<TKey, TValue> src, TKey key, Func<Task<TValue>> asyncValueResolver) where TKey: class                                                                                                                                                      where TValue: class?
        {
            asyncValueResolver = asyncValueResolver ?? throw new ArgumentNullException(nameof(asyncValueResolver));
            src = src ?? throw new ArgumentNullException(nameof(asyncValueResolver));

            // Value is already computed, return it
            if (src.TryGetValue(key, out var value))
            {
                return value;
            }

            // Retrieve the attached waithandle collections (to store execution status for each call)
            var attachedWaitHandle = src.GetCustomProperty<ConcurrentDictionary<TKey, ManualResetEventSlim>>("_linkedPendingOperations");
            if (attachedWaitHandle is null)
            {
                attachedWaitHandle = new ConcurrentDictionary<TKey, ManualResetEventSlim>();
                src.SetCustomProperty("_linkedPendingOperations", attachedWaitHandle);
            }

            // Value is already being comptued, add a listener to be warned when computation is done
            if (attachedWaitHandle.TryGetValue(key, out var eventSlim))
            {
                eventSlim.Wait();

                if (src.TryGetValue(key, out value))
                {
                    if (attachedWaitHandle.TryRemove(key, out eventSlim))
                    {
                        eventSlim.Dispose();
                    }

                    return value;
                }
            }

            // Value has to be computed, call the resolver 
            attachedWaitHandle.TryAdd(key, new ManualResetEventSlim(false));

            value = await asyncValueResolver().ConfigureAwait(false);

            src.Add(key, value);

            attachedWaitHandle[key].Set();

            return value;
        }

    }
}
#nullable disable
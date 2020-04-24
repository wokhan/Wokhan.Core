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
    public static class DictionaryExtensions
    {
        public static void AddIfValued(this IDictionary propertyCollection, string propertyName, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                propertyCollection[propertyName] = value;
            }
        }

        public static void AddIfValued(this IDictionary propertyCollection, string propertyName, bool value)
        {
            if (value)
            {
                propertyCollection[propertyName] = value;
            }
        }

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
            // Since we are using "custom properties" (relying on weakdictionary
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
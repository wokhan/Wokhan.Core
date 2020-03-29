#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Wokhan.Collections
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged where TKey: class 
                                                                                                         where TValue : class?
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public bool TryGetValue(TKey key, out TValue value, Action<TValue>? callback = null)
        {
            if (TryGetValue(key, out value, callback))
            {
                // Object found and not null
                if (value is object)
                {
                    callback?.Invoke(value);
                    return true;
                }

                // Or object is null, meaning loading is still in progress
                void add(object sender, NotifyCollectionChangedEventArgs args)
                {
                    if (args.Action == NotifyCollectionChangedAction.Replace)
                    {
                        var entry = (KeyValuePair<TKey, TValue>)args.NewItems[0];
                        if (key.Equals(entry.Key))
                            callback?.Invoke(entry.Value);
                    }
                }
                CollectionChanged += add;
                CollectionChanged += (s, e) => CollectionChanged -= add;
            }
            
            return false;
        }


        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
        }

        public new void Remove(TKey key)
        {
            base.Remove(key);

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Refresh()
        {
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        }

        public new void Clear()
        {
            base.Clear();

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new TValue this[TKey key]
        {
            get { return base[key]; }
            set { var oldvalue = base[key]; base[key] = value; NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, oldvalue))); }
        }

        protected virtual void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}
#nullable disable

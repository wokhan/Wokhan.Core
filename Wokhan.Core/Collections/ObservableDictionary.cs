﻿using System.Collections.Generic;
using System.Collections.Specialized;

namespace Wokhan.Collections;

/// <summary>
/// An observable variant of <see cref="Dictionary{TKey, TValue}"/>.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged where TKey : class
                                                                                                     where TValue : class?
{

    public event NotifyCollectionChangedEventHandler? CollectionChanged;


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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Wokhan.Collections.Generic.Extensions;
using Wokhan.ComponentModel.Extensions;

namespace Wokhan.Collections;

/// <summary>
/// Custom observable collection with grouping abilities
/// </summary>
/// <typeparam name="TK">Groups key type</typeparam>
/// <typeparam name="T">Items type</typeparam>
public class GroupedObservableCollection<TK, T> : ObservableCollection<ObservableGrouping<TK, T>>, INotifyPropertyChanged
{
    protected override event PropertyChangedEventHandler PropertyChanged;

    private bool _loading;
    /// <summary>
    /// Indicates if data is still loading
    /// </summary>
    public bool Loading
    {
        get => _loading;
        private set => this.SetValue(ref _loading, value, RaisePropertyChanged);
    }

    public void RaisePropertyChanged([CallerMemberName] string? prop = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    /// <summary>
    /// Returns all keys for this dictionary
    /// </summary>
    public IEnumerable<TK> Keys => this.Select(x => x.Key);

    /// <summary>
    /// Returns all values for this dictionary
    /// </summary>
    public IEnumerable<T> Values => this.SelectMany(x => x);

    private readonly Func<T, TK> keyGetter;

    /// <summary>
    /// Builds a new <see cref="GroupedObservableCollection{TK, T}"/>, grouping by the "keyGetter" specifier.
    /// </summary>
    /// <param name="keyGetter">Method to retrieve the key to group values on</param>
    /// <param name="initialKeys">Initial list of key values</param>
    public GroupedObservableCollection(Func<T, TK> keyGetter, List<TK>? initialKeys = null)
    {
        this.keyGetter = keyGetter;
        if (initialKeys is not null)
        {
            this.AddRange(initialKeys.Select(key => new ObservableGrouping<TK, T>(key)));
        }
    }

    /// <summary>
    /// Adds an item to the list, optionnally inserting it at the position defined by the <paramref name="orderBy"/> parameter.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="orderBy">Method defining the order to take into account when inserting the item</param>
    public void Add(T item, Func<T, IComparable>? orderBy = null)
    {
        TK key = keyGetter(item);
        ObservableGrouping<TK, T> group = this.FirstOrDefault(x => x.Key.Equals(key));
        if (group is null)
        {
            group = new ObservableGrouping<TK, T>(key);
            this.Add(group);
        }

        if (orderBy is not null)
        {
            group.InsertOrdered(item, orderBy(item), orderBy);
        }
        else
        {
            group.Add(item);
        }
    }

    /// <summary>
    /// Indicates that data loading begins
    /// </summary>
    public void BeginInit()
    {
        Loading = true;
    }

    /// <summary>
    /// Indicates that data loading ends
    /// </summary>
    public void EndInit()
    {
        Loading = false;
    }

}

/// <summary>
/// IGrouping specialization used by <see cref="GroupedObservableCollection{TK, T}"/>
/// </summary>
/// <typeparam name="TK">Groups key type</typeparam>
/// <typeparam name="T">Item type</typeparam>
public class ObservableGrouping<TK, T> : ObservableCollection<T>, IGrouping<TK, T>, IObservableGrouping<TK>
{
    public TK Key { get; }

    public ObservableGrouping(TK key)
    {
        this.Key = key;
    }

    public ObservableGrouping(TK key, IEnumerable<T> items) : this(key)
    {
        this.AddRange(items);
    }
}

public interface IObservableGrouping<T>
{
    T Key { get; }
}
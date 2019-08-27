using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Wokhan.Collections.Generic.Extensions;
using Wokhan.ComponentModel.Extensions;

namespace Wokhan.Collections
{
    public class GroupedObservableCollection<TK, T> : ObservableCollection<ObservableGrouping<TK, T>>, INotifyPropertyChanged
    {
        protected override event PropertyChangedEventHandler PropertyChanged;

        private bool _loading;
        public bool Loading
        {
            get => _loading;
            private set => this.SetValue(ref _loading, value, RaisePropertyChanged);
        }

        public void RaisePropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public IEnumerable<TK> Keys => this.Select(x => x.Key);

        public IEnumerable<T> Values => this.SelectMany(x => x);

        private readonly Func<T, TK> keyGetter;

        public GroupedObservableCollection(Func<T, TK> keyGetter, List<TK> initialKeys = null)
        {
            this.keyGetter = keyGetter;
            if (initialKeys != null)
            {
                this.AddRange(initialKeys.Select(key => new ObservableGrouping<TK, T>(key)));
            }
        }

        public void Add(T item, Func<T, IComparable> orderBy = null)
        {
            TK key = keyGetter(item);
            ObservableGrouping<TK, T> group = this.FirstOrDefault(x => x.Key.Equals(key));
            if (group == null)
            {
                group = new ObservableGrouping<TK, T>(key);
                this.Add(group);
            }

            if (orderBy != null)
            {
                group.InsertOrdered(item, orderBy(item), orderBy);
            }
            else
            {
                group.Add(item);
            }
        }

        public void BeginInit()
        {
            Loading = true;
        }

        public void EndInit()
        {
            Loading = false;
        }

    }

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
}
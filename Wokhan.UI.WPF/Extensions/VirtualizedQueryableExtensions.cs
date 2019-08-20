using AlphaChiTech.Virtualization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Wokhan.WPF.Extensions
{
    public static class VirtualizedQueryableExtensions
    {
        private static Dictionary<Type, Type> cachedTypes = new Dictionary<Type, Type>();
        
        public static IEnumerable AsVirtualized(this IQueryable query)
        {
            Type t = query.ElementType;
            Type gt;
            if (!cachedTypes.TryGetValue(t, out gt))
            {
                gt = typeof(TypedVirtualSource<>).MakeGenericType(t);

                cachedTypes.Add(t, gt);
            }

            return (IEnumerable)Activator.CreateInstance(gt, query);
        }

        public class TypedVirtualSource<T> : VirtualizingObservableCollection<T>
        {
            public TypedVirtualSource(IOrderedQueryable query)
                : base(new PaginationManager<T>(new Source((IOrderedQueryable<T>)query)))
            {

            }


            public class Source : IPagedSourceProvider<T>
            {
                IOrderedQueryable<T> basequery;

                public Source(IOrderedQueryable<T> query)
                {
                    basequery = query;
                }

                public PagedSourceItemsPacket<T> GetItemsAt(int pageoffset, int count, bool usePlaceholder)
                {
                    var ret = new PagedSourceItemsPacket<T>();

                    ret.LoadedAt = DateTime.Now;
                    ///ret.Items = DLinq.DynamicQueryableExtensions.OrderBy(basequery ?? (IQueryable<T>)prv.GetQueryable(src), prv.GetColumns(src).First().Name).Skip(pageoffset).Take(count).AsEnumerable();
                    ret.Items = basequery.Skip(pageoffset).Take(count).AsEnumerable();

                    return ret;
                }

                public int Count
                {
                    get { return basequery.Count(); }
                }

                public int IndexOf(T item)
                {
                    return basequery.ToList().IndexOf(item);
                }

                public void OnReset(int count)
                {
                }

                public async Task<int> GetCountAsync()
                {
                    return await Task.Run(() => Count);
                }

                public async Task<PagedSourceItemsPacket<T>> GetItemsAtAsync(int pageoffset, int count, bool usePlaceholder)
                {
                    return await Task.Run(() => GetItemsAt(pageoffset, count, usePlaceholder));
                }

                public T GetPlaceHolder(int index, int page, int offset)
                {
                    return GetItemsAt(0, 1, false).Items.First();
                }

                public async Task<int> IndexOfAsync(T item)
                {
                    return await Task.Run(() => IndexOf(item));
                }
            }
        }

        public static void Init(Dispatcher dispatcher)
        {
            if (!VirtualizationManager.IsInitialized)
            {
                VirtualizationManager.Instance.UIThreadExcecuteAction = (a) => dispatcher.Invoke(a);
                new DispatcherTimer(TimeSpan.FromSeconds(1),
                                    DispatcherPriority.Background,
                                    (s, a) => VirtualizationManager.Instance.ProcessActions(),
                                    dispatcher).Start();
            }
        }
    }
}

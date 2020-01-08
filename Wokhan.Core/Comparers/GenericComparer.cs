using System;
using System.Collections.Generic;

namespace Wokhan.Core.Comparers 
{
    public class GenericComparer<T> : IEqualityComparer<T>
    {
        private Func<T, object> keyGetter;

        public GenericComparer(Func<T, object> keyGetter) => this.keyGetter = keyGetter;
        public bool Equals(T x, T y) => keyGetter(x)?.Equals(keyGetter(y)) ?? false;
        public int GetHashCode(T obj) => keyGetter(obj)?.GetHashCode() ?? 0;
    }
}
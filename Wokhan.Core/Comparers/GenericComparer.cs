using System;
using System.Collections.Generic;

namespace Wokhan.Core.Comparers 
{
    public class GenericComparer<T, TK> : IEqualityComparer<T>
    {
        private Func<T, TK> keyGetter;

        public GenericComparer(Func<T, TK> keyGetter) => this.keyGetter = keyGetter;
        public bool Equals(T x, T y) => keyGetter(x)?.Equals(keyGetter(y)) ?? false;
        public int GetHashCode(T obj) => keyGetter(obj)?.GetHashCode() ?? 0;
    }
}
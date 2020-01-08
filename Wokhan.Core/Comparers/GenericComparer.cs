using System;
using System.Collections.Generic;

namespace Wokhan.Core.Comparers 
{
    public class GenericComparer<T> : EqualityComparer<T>
    {
        private Func<T, object> keyGetter;

        public GenericComparer(Func<T, object> keyGetter) => this.keyGetter = keyGetter;
        public override bool Equals(T x, T y) => keyGetter(x)?.Equals(keyGetter(y)) ?? false;
        public override int GetHashCode(T obj) => keyGetter(obj)?.GetHashCode() ?? 0;
    }
}
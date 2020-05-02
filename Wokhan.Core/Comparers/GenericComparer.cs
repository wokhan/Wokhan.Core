using System;
using System.Collections.Generic;

namespace Wokhan.Core.Comparers 
{
    /// <summary>
    /// Generic equality comparer, to quickly define custom comparer 
    /// </summary>
    /// <typeparam name="T">Type of the objects to compare</typeparam>
    public class GenericComparer<T> : EqualityComparer<T>
    {
        private Func<T, object> keyGetter;

        /// <summary>
        /// Creates a new <see cref="GenericComparer{T}"/> using the specified key getter
        /// </summary>
        /// <param name="keyGetter">Method used to retrieve the value (key) to compare on</param>
        public GenericComparer(Func<T, object> keyGetter) => this.keyGetter = keyGetter;

        /// <summary>
        /// Check two items keys (as retrieved by the key getter) for equality
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals(T x, T y) => keyGetter(x)?.Equals(keyGetter(y)) ?? false;

        /// <summary>
        /// Returns the hash code of the key as retrieved by the key getter method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int GetHashCode(T obj) => keyGetter(obj)?.GetHashCode() ?? 0;
    }
}
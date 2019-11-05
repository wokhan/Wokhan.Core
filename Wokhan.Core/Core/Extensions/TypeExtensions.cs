using System;
using System.Reflection;

namespace Wokhan.System.Extensions
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type t)
        {
            return ((Func<object>)GetDefaultGeneric<object>).Method.GetGenericMethodDefinition().MakeGenericMethod(t).Invoke(null, null);
        }

        public static T GetDefaultGeneric<T>()
        {
            return default(T);
        }

        public static T AnonymousToKnownType<T>(this object o) where T : class
        {
            return (T)o;
        }

        public static Delegate GetDelegateForPrivate(this Type type, Type type2, object obj, string method)
        {
            return Delegate.CreateDelegate(type2, obj, type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance));
        }
    }
}

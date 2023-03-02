using System;
using System.Reflection;

namespace Wokhan.Core.Extensions;

/// <summary>
/// Type class extensions methods
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Returns the default value for a dynamic type (not know at runtime)
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static object GetDefault(this Type t)
    {
        return ((Func<object>)GetDefaultGeneric<object>).Method.GetGenericMethodDefinition().MakeGenericMethod(t).Invoke(null, null);
    }

    /// <summary>
    /// Returns the default value for a dynamic type (not know at runtime)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetDefaultGeneric<T>()
    {
        return default;
    }

    /// <summary>
    /// Casts an anonymously typed object to a known type (which must have the same properties, of course)
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <param name="o">Source dynamicobject</param>
    /// <returns>The same object but with the target type</returns>
    public static T AnonymousToKnownType<T>(this object o) where T : class
    {
        return (T)o;
    }

    /// <summary>
    /// Creates a delegate to be able to call private methods on the given type.
    /// Use with caution, methods are private for a reason...
    /// </summary>
    /// <param name="type">Souce Type to create delegate for</param>
    /// <param name="returnType">Return type of the delegate</param>
    /// <param name="obj">Object to get the delegate for</param>
    /// <param name="method">Name of the private method to create the delegate for</param>
    /// <returns></returns>
    public static Delegate GetDelegateForPrivate(this Type type, Type returnType, object obj, string method)
    {
        return Delegate.CreateDelegate(returnType, obj, type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance));
    }
}

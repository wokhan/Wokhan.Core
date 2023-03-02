using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Wokhan.Core.Extensions;

/// <summary>
/// Extensions for all objects
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Returns a single object as a singleton array
    /// Note: Naming looks wrong. This method will probably be removed in a later release.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T[] AsArray<T>(this T obj)
    {
        return new T[] { obj };
    }

    /// <summary>
    /// Recursively retrieves a value from a deep property for the given object
    /// <code>
    /// myObject.GetValueFromPath("Property.PropertyProperty") returns myObject.Property.PropertyProperty
    /// </code>
    /// </summary>
    /// <param name="o">Source object</param>
    /// <param name="path">Path to the property (dot separated)</param>
    /// <returns></returns>
    public static object? GetValueFromPath(this object? o, string path)
    {
        if (o is null)
        {
            return null;
        }

        if (path == ".")
        {
            return o;
        }

        Type type = o.GetType();
        var props = path.Split('.');
        var current = o;
        foreach (var prop in props)
        {
            current = type.GetProperty(prop).GetValue(current);
            if (current is null)
            {
                break;
            }
            type = current.GetType();
        }

        return current;
    }

    /// <summary>
    /// Tries to convert an object to a target type, handling nulls, DBNull and empty strings.
    /// </summary>
    /// <param name="o">Source object</param>
    /// <param name="targetType">Target type</param>
    /// <returns></returns>
    public static object? SafeConvert(this object? o, Type targetType)
    {
        Contract.Requires(targetType is not null);

        if (o is DBNull || o is null || (o is string && String.IsNullOrEmpty((string)o)))
        {
            return null;
        }
        else
        {
            Type aType = o.GetType();
            Type t = Nullable.GetUnderlyingType(aType);

            object? safeValue;
            if (t is not null)
            {
                safeValue = (o is null || o == DBNull.Value) ? null : Convert.ChangeType(o, t);
            }
            else
            {
                safeValue = o;
            }

            if (targetType.IsGenericType &&
              targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (safeValue is null)
                {
                    return null;
                }

                targetType = new NullableConverter(targetType).UnderlyingType;
            }

            return Convert.ChangeType(safeValue, targetType, CultureInfo.InvariantCulture);
        }
    }

    // Set to Internal to allow access from Test project
    internal static ConditionalWeakTable<object, Dictionary<string, object?>> WeakTable { get; } = new();

    /// <summary>
    /// WARNING: PROTOTYPE. Do not use until this notice is removed since memory impact has not been verified yet.
    /// Stores a custom property attached to the source object
    /// The dictionary storing data will be garbage collected once the object is not used anymore, preventing memory leaks
    /// <seealso cref="ConditionalWeakTable{TKey, TValue}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetCustomProperty<T>(this object src, string key, T? value)
    {
        src = src ?? throw new ArgumentNullException(nameof(src));
        key = key ?? throw new ArgumentNullException(nameof(key));

        WeakTable.GetOrCreateValue(src)[key] = value;
    }

    /// <summary>
    /// WARNING: PROTOTYPE. Do not use until this notice is removed since memory impact has not been verified yet.
    /// Retrieves an "attached" property for the source object
    /// The dictionary storing data will be garbage collected once the object is not used anymore, preventing memory leaks
    /// <seealso cref="ConditionalWeakTable{TKey, TValue}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src"></param>
    /// <param name="key"></param>
    /// <param name="defaultIfNotFound"></param>
    /// <returns></returns>
    public static T? GetCustomProperty<T>(this object src, string key, T? defaultIfNotFound = default)
    {
        src = src ?? throw new ArgumentNullException(nameof(src));
        key = key ?? throw new ArgumentNullException(nameof(key));

        return WeakTable.TryGetValue(src, out var dic) && dic.TryGetValue(key, out var ret) ? (T?)ret : defaultIfNotFound;
    }
}

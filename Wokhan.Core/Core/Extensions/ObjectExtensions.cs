using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Wokhan.System.Extensions
{
    public static class ObjectExtensions
    {
        public static T[] AsArray<T>(this T obj)
        {
            return new T[] { obj };
        }

        public static Object GetValueFromPath(this Object obj, string path)
        {
            if (obj == null)
            {
                return null;
            }

            if (path == ".") 
            {
                return obj;
            }
            
            Type type = obj.GetType();
            var props = path.Split('.');
            var o = obj;
            foreach (var prop in props)
            {
                o = type.GetProperty(prop).GetValue(o);
                if (o == null)
                {
                    break;
                }
                type = o.GetType();
            }

            return o;
        }

        public static object SafeConvert(this object a, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (a is DBNull || a == null || (a is string && String.IsNullOrEmpty((string)a)))
            {
                return null;
            }
            else
            {
                Type aType = a.GetType();
                Type t = Nullable.GetUnderlyingType(aType);

                object safeValue;
                if (t != null)
                {
                    safeValue = (a == null || a == DBNull.Value) ? null : Convert.ChangeType(a, t);
                }
                else
                {
                    safeValue = a;
                }

                if (targetType.IsGenericType &&
                  targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    if (safeValue == null)
                    {
                        return null;
                    }

                    targetType = new NullableConverter(targetType).UnderlyingType;
                }

                return Convert.ChangeType(safeValue, targetType, CultureInfo.InvariantCulture);
            }
        }

        private static ConditionalWeakTable<object, Dictionary<string, object>> _weakTable = new ConditionalWeakTable<object, Dictionary<string, object>>();
        public static void SetCustomProperty<T>(this object src, string key, T value)
        {
            if (!_weakTable.TryGetValue(src, out var dic))
            {
                _weakTable.Add(src, new Dictionary<string, object> { [key] = value });
            }
            else
            {
                dic[key] = value;
            }
        }

        public static T GetCustomProperty<T>(this object src, string key)
        {
            object ret = null;
            if (_weakTable.TryGetValue(src, out var dic))
            {
                dic.TryGetValue(key, out ret);
            }

            return (T)ret;
        }
    }
}
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Wokhan.ComponentModel.Extensions
{
    public static class NotifyPropertyChangedExtensions
    {
        public static void SetValue<T>(this INotifyPropertyChanged src, ref T field, T value, Action<string> propertyChanged = null, [CallerMemberName] string propertyName = null)
        {
            if (field == null || !field.Equals(value))
            {
                field = value;
                propertyChanged?.Invoke(propertyName);
            }
        }
    }
}
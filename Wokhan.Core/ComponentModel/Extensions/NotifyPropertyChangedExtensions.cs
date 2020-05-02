using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Wokhan.ComponentModel.Extensions
{
    /// <summary>
    /// Extensions for <see cref="INotifyPropertyChanged"/> implementers
    /// </summary>
    public static class NotifyPropertyChangedExtensions
    {
        /// <summary>
        /// Sets a value, taking care of the <see cref="PropertyChangedEventHandler"/> invocation if value did change.
        /// <code>
        /// private bool _loading;
        /// public bool Loading
        /// {
        ///    get => _loading;
        ///    private set => this.SetValue(ref _loading, value, RaisePropertyChanged);
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src">Ignored parameter (only used to allow this method to be used as an extension on <see cref="INotifyPropertyChanged"/> implementers</param>
        /// <param name="field">Field to set</param>
        /// <param name="value">Value to set the field to</param>
        /// <param name="propertyChanged">Handler</param>
        /// <param name="propertyName">"Injected" target property name (sent to the <paramref name="propertyChanged"/> handler)</param>
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
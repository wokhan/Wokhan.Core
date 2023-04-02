using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Wokhan.ComponentModel.Extensions;

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
    public static void SetValue<T>(this INotifyPropertyChanged src, ref T field, T value, Action<string>? propertyChanged = null, [CallerMemberName] string? propertyName = null)
    {
        if (field is null || !field.Equals(value))
        {
            field = value;
            propertyChanged?.Invoke(propertyName);
        }
    }

    /// <summary>
    /// Asynchronously retrieves a value and updates a backing field, calling the PropertyChanged handler when done.
    /// Allows to use lazy loading for costly operations in accessors, as follows:
    /// <code>
    /// private ImageSource? image;
    /// public ImageSource? Image 
    /// {
    ///     get => this.GetOrSetValueAsync(() => myAsyncMethod(), ref image, OnPropertyChanged);
    ///     set => image = value; // Optional (and usually useless since the value is set when the getter gets called for the first time)
    /// }
    /// </code>
    /// Note: if the getter is called multiple times while the computed value is null, the async callback will be called again. You should handle this to avoid this behavior.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src"></param>
    /// <param name="resolve"></param>
    /// <param name="targetField"></param>
    /// <param name="propertyChanged"></param>
    /// <param name="fieldName">For internal use (for reflection, based on the 'targetField' parameter)</param>
    /// <param name="propertyName">For internal use (for reflection, based on the 'targetField' parameter)</param>
    /// <returns></returns>
    public static T? GetOrSetValueAsync<T>(this INotifyPropertyChanged src, Func<T> resolve, ref T targetField, Action<string>? propertyChanged = null, [CallerArgumentExpression(nameof(targetField))] string? fieldName = null, [CallerMemberName] string? propertyName = null)
    {
        // TODO: not sure about that. Wrapping the resolver in a task wrapper in a function only to have it run async looks a bit too much.
        return GetOrSetValueAsync(src, () => Task.Run(() => resolve()), ref targetField, propertyChanged, propertyName);
    }

    /// <summary>
    /// Asynchronously retrieves a value and updates a backing field, calling the PropertyChanged handler when done.
    /// Allows to use lazy loading for costly operations in accessors, as follows:
    /// <code>
    /// private ImageSource? image;
    /// public ImageSource? Image 
    /// {
    ///     get => this.GetOrSetValueAsync(() => myAsyncMethod(), ref image, OnPropertyChanged);
    ///     set => image = value; // Optional (and usually useless since the value is set when the getter gets called for the first time)
    /// }
    /// </code>
    /// Note: if the getter is called multiple times while the computed value is null, the async callback will be called again. You should handle this to avoid this behavior.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src"></param>
    /// <param name="resolveAsync">Async factory to resolve the value</param>
    /// <param name="targetField"></param>
    /// <param name="propertyChanged"></param>
    /// <param name="fieldName">For internal use (for reflection, based on the 'targetField' parameter)</param>
    /// <param name="propertyName">For internal use (for reflection, based on the 'targetField' parameter)</param>
    /// <returns></returns>
    public unsafe static T? GetOrSetValueAsync<T>(this INotifyPropertyChanged src, Func<Task<T>> resolveAsync, ref T targetField, Action<string>? propertyChanged = null, [CallerArgumentExpression(nameof(targetField))] string? fieldName = null, [CallerMemberName] string? propertyName = null)
    {
        // Dangerous code as it doesn't ensure field has been pinned in memory, resulting in memory violations.
        // Keeping it in case someone has an idea.
        //if (field is not null)
        //{
        //    return field;
        //}

        //var ptr = Unsafe.AsPointer(ref field);
        //_ = resolveAsync().ContinueWith(task =>
        //{
        //    Unsafe.Write(ptr, task.Result);
        //    propertyChanged?.Invoke(propertyName);
        //});


        //return field;


        fieldName ??= $"<{propertyName}>k_BackingField"; // TODO: check how this is generated. Could be broken with some .NET future implementations.
        var fieldInfo = src.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo is null)
        {
            throw new ArgumentOutOfRangeException(fieldName);
        }
        if (targetField is null)
        {
            //TODO: check why we are using TaskScheduler.Current and not Default (might cause a thread issue)
            _ = resolveAsync().ContinueWith(task => { fieldInfo.SetValue(src, task.Result); propertyChanged?.Invoke(propertyName!); }, TaskScheduler.Current);
        }
        return targetField;
    }
}
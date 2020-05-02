using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Wokhan.Core.ComponentModel
{
    /// <summary>
    /// Simple helper to avoid implementing <see cref="INotifyPropertyChanged"/> every time.
    /// </summary>
    public class NotifierHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Event invocation (injects the property name using <see cref="CallerMemberNameAttribute"/> when used in setters, no need for nameof operator)
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

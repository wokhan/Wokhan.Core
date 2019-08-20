#if !__UAP__

using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Wokhan.UWP.Settings
{
    public partial class SettingsManager
    {
        private static void _cleanUp(bool usePublisherFolder, params string[] fileNames)
        {

        }
        
        private static async Task<T> LoadSettingsAsync<T>(string fileName, bool usePublisherFolder, Func<T> defaultValue = null)
        {
            return defaultValue != null ? defaultValue() : Activator.CreateInstance<T>();
        }

        private static async Task<bool> SaveSettingsAsync<T>(T calOpts, string fileName, bool usePublisher)
        {
            return false;
        }
    }
}

#endif
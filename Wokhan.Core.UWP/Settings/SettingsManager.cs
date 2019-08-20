using System;
using Windows.Foundation;

namespace Wokhan.UWP.Settings
{
    public partial class SettingsManager
    {
        public static void CleanUp(bool usePublisherFolder, params string[] fileNames)
        {
            _cleanUp(usePublisherFolder, fileNames);
        }

        public static IAsyncOperation<T> LoadSharedSettings<T>(string fileName = "Settings.dat", Func<T> defaultValue = null)
        {
            return LoadSettingsAsync<T>(fileName, true, defaultValue).AsAsyncOperation();
        }

        public static IAsyncOperation<T> LoadSettings<T>(string fileName = "Settings.dat", Func<T> defaultValue = null)
        {
            return LoadSettingsAsync<T>(fileName, false, defaultValue).AsAsyncOperation();
        }
        
        public static IAsyncOperation<bool> SaveSharedSettings<T>(T calOpts, string fileName = "Settings.dat")
        {
            return SaveSettingsAsync(calOpts, fileName, true).AsAsyncOperation();
        }

        public static IAsyncOperation<bool> SaveSettings<T>(T calOpts, string fileName = "Settings.dat")
        {
            return SaveSettingsAsync(calOpts, fileName, false).AsAsyncOperation();
        }
    }
}
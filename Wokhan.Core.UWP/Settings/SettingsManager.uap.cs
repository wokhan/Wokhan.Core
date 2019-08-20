#if __UAP__

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using System.Linq;

namespace Wokhan.UWP.Settings
{
    public partial class SettingsManager
    {
        private static readonly StorageFolder BaseSharedFolder = ApplicationData.Current.GetPublisherCacheFolder("SmartTiles");
        private static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;

        private static void _cleanUp(bool usePublisherFolder, params string[] fileNames)
        {
            StorageFolder baseFolder = usePublisherFolder ? BaseSharedFolder : LocalFolder;
            fileNames.ToList().ForEach(async f =>
            {
                try
                { 
                    await (await baseFolder.GetFileAsync(f)).DeleteAsync();
                } catch (FileNotFoundException) { }
            });
        }

        private static async Task<T> LoadSettingsAsync<T>(string fileName, bool usePublisherFolder, Func<T> defaultValue = null)
        {
            StorageFolder baseFolder = usePublisherFolder ? BaseSharedFolder : LocalFolder;

            var sets = (StorageFile)await baseFolder.TryGetItemAsync(fileName);

            if (sets == null && usePublisherFolder)
            {
                sets = (StorageFile)await LocalFolder.TryGetItemAsync(fileName);
                // Moving the file back to the publisher folder (as we wanted to use it)
                if (sets != null)
                {
                    sets = await sets.CopyAsync(baseFolder);
                }
            }

            if (sets != null)
            {
                try
                {
                    using (Stream r = await sets.OpenStreamForReadAsync())
                    {
                        return (T)new DataContractSerializer(typeof(T)).ReadObject(r);
                    }
                }
                catch (Exception e)
                {
                    await sets.CopyAsync(baseFolder, fileName + ".bk", NameCollisionOption.ReplaceExisting);
                    throw e;
                }
            }

            return defaultValue != null ? defaultValue() : Activator.CreateInstance<T>();
        }

        private static async Task<bool> SaveSettingsAsync<T>(T calOpts, string fileName, bool usePublisher)
        {
            StorageFile sets = await (usePublisher ? BaseSharedFolder : LocalFolder).CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (Stream r = await sets.OpenStreamForWriteAsync())
            {
                new DataContractSerializer(typeof(T)).WriteObject(r, calOpts);
            }
            return true;
        }
    }
}

#endif
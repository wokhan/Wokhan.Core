using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Wokhan.UWP.Storage.Extensions
{
    public static class StorageFileExtensions
    {
        public static async Task SaveThumbnail(this StorageFile file, string localFileName)
        {
#if __UAP__
            StorageItemThumbnail pic = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.DocumentsView);

            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(pic);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            StorageFile fileDest = await ApplicationData.Current.LocalFolder.CreateFileAsync(localFileName, CreationCollisionOption.ReplaceExisting);
            using (Windows.Storage.Streams.IRandomAccessStream stream = await fileDest.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                await encoder.FlushAsync();
            }
#endif
        }
    }
}

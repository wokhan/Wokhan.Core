using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Wokhan.UWP.Extensions
{
    public static class UIElementExtensions
    {
        private static readonly DisplayInformation displayInfo = DisplayInformation.GetForCurrentView();

        /*public static IAsyncOperation<bool> RenderToPngAsync(this UIElement lt, string filename)
        {
            return RenderToPngAsync(lt, filename).AsAsyncOperation();
        } */

        public static async Task<bool> RenderToPngAsync(this UIElement lt, string filename)
        {
#if __UAP__
            var y = new RenderTargetBitmap();
            await y.RenderAsync(lt);
            Windows.Storage.Streams.IBuffer px = await y.GetPixelsAsync();

            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                using (Windows.Storage.Streams.IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.IsThumbnailGenerated = false;
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)y.PixelWidth, (uint)y.PixelHeight, displayInfo.RawDpiX, displayInfo.RawDpiY, px.ToArray());
                    await encoder.FlushAsync();
                }
                await CachedFileManager.CompleteUpdatesAsync(file);

                return true;
            }
#endif
            return false;
        }
    }
}

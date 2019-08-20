using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Wokhan.UWP.UI.Xaml.Media.Extensions
{
    public static class ImageSourceExtensions
    {
        public static async Task<BitmapImage> LoadFromLocalStorage(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            try
            {
#if __UAP__
                IRandomAccessStreamWithContentType f = await (await ApplicationData.Current.LocalFolder.GetFileAsync(name)).OpenReadAsync();
                var bmp = new BitmapImage();
                bmp.SetSource(f);
                return bmp;
#else
                return null;
#endif
            }
            catch
            {
                return null;
            }
        }

        /*public static async Task<ImageSource> LoadFromResources(string path)
        {
            var str = RandomAccessStreamReference.CreateFromUri(new Uri(path));

        }*/

        public static async Task SaveToLocalStorage(this ImageSource src, string name)
        {
#if WINDOWS_UWP
            Uri uriSource = null;
            if (src is SvgImageSource svg)
            {
                uriSource = svg.UriSource;
            }
            else if (src is BitmapImage bmp)
            {
                uriSource = bmp.UriSource;
            }

            if (uriSource == null)
            {
                throw new ArgumentOutOfRangeException(nameof(src));
            }

            StorageFile fileDest = await ApplicationData.Current.LocalFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStreamWithContentType stream = await RandomAccessStreamReference.CreateFromUri(uriSource).OpenReadAsync())
            {
                using (IRandomAccessStream targetStream = await fileDest.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await RandomAccessStream.CopyAndCloseAsync(stream, targetStream);
                }
            }
#endif
        }
    }
}

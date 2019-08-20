using System;
using System.Threading.Tasks;
#if __UAP__
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
#endif
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Wokhan.UWP.UI.Extensions
{
    public static class UIExtensions
    {
        public static readonly DependencyProperty SecondaryContentProperty = DependencyProperty.RegisterAttached("SecondaryContent", typeof(object), typeof(UIExtensions), new PropertyMetadata(null, SecondaryContentPropertyChange));

        private static void SecondaryContentPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public static object GetSecondaryContent(FrameworkElement lst)
        {
            return lst.GetValue(SecondaryContentProperty);
        }

        public static void SetSecondaryContent(FrameworkElement lst, object value)
        {
            lst.SetValue(SecondaryContentProperty, value);
        }

        public static T FindOrAdd<T>(this Panel parent, string name, int position = -1) where T : FrameworkElement, new()
        {
            var panel = (T)parent.FindName(name);
            if (panel == null)
            {
                panel = new T() { Name = name };
                if (position == -1)
                {
                    parent.Children.Add(panel);
                }
                else
                {
                    parent.Children.Insert(position, panel);
                }
            }

            return panel;
        }

        public static async Task LaunchInNewWindow<T>(object args, string title, Action callback = null, double width = -1, double height = -1) where T : Page
        {
#if __UAP__
            CoreApplicationView newAV = CoreApplication.CreateNewView();
            var appV = ApplicationView.GetForCurrentView();

            await newAV.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var newAppView = ApplicationView.GetForCurrentView();
                newAppView.Title = title;

                if (width != -1 && height != -1)
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
                    ApplicationView.PreferredLaunchViewSize = new Size(width, height);
                    if (width < 500 || height < 320)
                    {
                        newAppView.SetPreferredMinSize(new Size(width, height));
                    }
                }

                var frame = new Frame();
                frame.Navigate(typeof(T), args);

                Window.Current.Content = frame;
                Window.Current.Activate();

                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppView.Id, ViewSizePreference.Default, appV.Id, ViewSizePreference.Default);

                callback?.Invoke();
            });
#endif
        }

        /*
        public async static Task<ContentDialogResult> LaunchInContentDialog<T>(object args, string title, double width = 500, double height = 400) where T : Page
        {
            //Doesn't look like it wants to crash this time ;-)
            var cd = new ContentDialog()
            {
                Title = title,
                Content = Activator.CreateInstance(typeof(T), args),
                Width = width,
                Height = height,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                Background = new SolidColorBrush(Colors.Transparent)
            };

            var ret = await cd.ShowAsync();

            return ret;
        }
        */


    }
}

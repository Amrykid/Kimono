using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Kimono.Controls.SnackBar
{
    public static class SnackBarManager
    {
        public static async Task ShowMessageAsync(string message, int msToBeVisible = 10000)
        {
            SnackBarMessage msgControl = new SnackBarMessage();
            msgControl.Text = message;

            await DoPopupAsync(msToBeVisible, msgControl);
        }

        private static async Task DoPopupAsync(int msToBeVisible, SnackBarMessage msgControl)
        {
            Popup popup = new Popup();
            popup.HorizontalAlignment = HorizontalAlignment.Left;
            popup.VerticalAlignment = VerticalAlignment.Top;

            popup.Child = msgControl;

            var rootElement = FindChildOfType<Grid>(Window.Current.Content);

            double heightPercentage = .85;
            Func<double> getHorizonalPos = () =>
            {
                var halfWidth = (Window.Current.Bounds.Width * .5);
                var halfPopupWidth = msgControl.ActualWidth / 2.0;

                return halfWidth - halfPopupWidth;
            };

            WindowSizeChangedEventHandler handler = null;
            handler = new WindowSizeChangedEventHandler((object sender, WindowSizeChangedEventArgs e) =>
            {
                popup.HorizontalOffset = getHorizonalPos();
                popup.VerticalOffset = Window.Current.Bounds.Height * heightPercentage;
            });

            Window.Current.SizeChanged += handler;

            if (rootElement is Grid)
            {
                rootElement.Children.Add(popup);
            }

            popup.IsOpen = true;
            popup.HorizontalOffset = getHorizonalPos() - (MeasureString(msgControl.Text, new Size(Window.Current.Bounds.Width, Window.Current.Bounds.Height), msgControl.FontSize).Width * 3.5);
            popup.VerticalOffset = Window.Current.Bounds.Height * heightPercentage;
            popup.IsOpen = false;

            popup.IsOpen = true;

            await Task.Delay(msToBeVisible);

            Window.Current.SizeChanged -= handler;

            if (rootElement is Grid)
            {
                ((Grid)rootElement).Children.Remove(popup);
            }
        }

        private static T FindChildOfType<T>(UIElement element) where T : UIElement
        {
            if (element is T) return (T)element;

            if (element is ContentControl)
            {
                if (((ContentControl)element).Content != null)
                    return FindChildOfType<T>(((ContentControl)element).Content as UIElement);
            }
            else if (element is Page)
            {
                if (((Page)element).Content != null)
                    return FindChildOfType<T>(((Page)element).Content as UIElement);
            }

            return null;
        }

        private static Size MeasureString(string content, Size availableSize, double fontSize, string fontFamily = "Segoe UI")
        {
            //https://social.msdn.microsoft.com/Forums/windowsapps/en-US/a05a194a-3f8d-474d-b6a1-f8eeb3914e72/uwp-how-do-i-measure-a-space-character?forum=wpdevelop

            TextBlock tb = new TextBlock();

            tb.TextWrapping = TextWrapping.Wrap;
            tb.Text = content;
            tb.FontFamily = new Windows.UI.Xaml.Media.FontFamily(fontFamily);
            tb.FontSize = fontSize;
            tb.Measure(availableSize);

            Size actualSize = new Size();
            actualSize.Width = tb.ActualWidth;
            actualSize.Height = tb.ActualHeight;

            //return tb.DesiredSize;
            return actualSize;
        }
    }
}

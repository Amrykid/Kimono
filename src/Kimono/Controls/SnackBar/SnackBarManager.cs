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
    public class SnackBarManager
    {
        private Grid desiredSnackBarAreaGrid  = null;
        public SnackBarManager(Grid snackBarAreaGrid)
        {
            desiredSnackBarAreaGrid = snackBarAreaGrid;
        }
        public async Task ShowMessageAsync(string message, int msToBeVisible = 10000)
        {
            SnackBarMessage msgControl = new SnackBarMessage();
            msgControl.Text = message;
            msgControl.TimeToShow = msToBeVisible;

            await DoPopupAsync(msgControl);
        }

        public async Task ShowMessageWithCallbackAsync(string message, string buttonText, Action<SnackBarMessage> callback, int msToBeVisible = 10000)
        {
            SnackBarMessage msgControl = new SnackBarMessage();
            msgControl.Text = message;
            msgControl.TimeToShow = msToBeVisible;

            //too lazy for dependency properties
            msgControl.ButtonVisibility = Visibility.Visible;
            msgControl.ButtonText = buttonText;
            msgControl.ButtonCallback = callback;

            await DoPopupAsync(msgControl);
        }

        private async Task DoPopupAsync(SnackBarMessage msgControl)
        {
            if (desiredSnackBarAreaGrid.Children.Any(x => x is SnackBarMessage))
            {
                //queue up

                var previousMsg = desiredSnackBarAreaGrid.Children.First(x => x is SnackBarMessage && x != msgControl) as SnackBarMessage;

                if (previousMsg.NextSnackBarMessage == null)
                {
                    previousMsg.NextSnackBarMessage = msgControl;
                    return;
                }
            }

            desiredSnackBarAreaGrid.Children.Add(msgControl);

            await Task.Delay(msgControl.TimeToShow); //todo check for 0

            if (msgControl.NextSnackBarMessage != null)
                DoPopupAsync(msgControl.NextSnackBarMessage); //recipe for disaster

            desiredSnackBarAreaGrid.Children.Remove(msgControl);
        }

        private T FindChildOfType<T>(UIElement element) where T : UIElement
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

        private Size MeasureString(string content, Size availableSize, double fontSize, string fontFamily = "Segoe UI")
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

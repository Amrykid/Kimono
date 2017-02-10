using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kimono.Controls
{
    public sealed class OverlayPanelManager
    {
        private Grid overlayAreaGrid = null;
        private IOverlayPanelScreen currentScreen = null;

        internal OverlayPanel ParentPanel { get; private set; }

        internal OverlayPanelManager(OverlayPanel parentPanel, Grid overlayArea)
        {
            ParentPanel = parentPanel;
            overlayAreaGrid = overlayArea;
        }

        private void VerifyUIThread()
        {
            if (!Window.Current.Dispatcher.HasThreadAccess)
                throw new InvalidOperationException("This method must be called on the UI thread.");
        }

        public void ShowScreen(IOverlayPanelScreen screen)
        {
            VerifyUIThread();

            if (screen == null) throw new ArgumentNullException(nameof(screen));

            if (!(screen is FrameworkElement))
                throw new ArgumentException(nameof(screen) + " must inherit either directly or indirectly from FrameworkElement.");

            if (currentScreen != null)
            {
                HideCurrentScreen();
            }

            ParentPanel.HideContent();

            currentScreen = screen;

            overlayAreaGrid.Children.Add(currentScreen as FrameworkElement);

            screen.OnShown(ParentPanel);
        }

        public void HideCurrentScreen()
        {
            VerifyUIThread();

            if (currentScreen != null)
            {
                currentScreen.OnClosed();

                overlayAreaGrid.Children.Remove(currentScreen as FrameworkElement);

                currentScreen = null;
            }

            ParentPanel.ShowContent();
        }

        public void ShowProgressOverlay()
        {
            ShowScreen(new ProgressOverlayScreen());
        }
    }
}

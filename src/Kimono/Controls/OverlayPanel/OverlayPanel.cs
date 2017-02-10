using Kimono.Controls.SnackBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Kimono.Controls
{
    public sealed class OverlayPanel : ContentControl
    {
        private SnackBarManager snackManager = null;
        private OverlayPanelManager overlayManager = null;
        private Grid PART_SnackBarGrid = null;
        private ContentPresenter PART_ContentPresenter = null;
        private Grid PART_OverlayGrid = null;

        public OverlayPanelManager OverlayProvider { get { return overlayManager; } }
        public SnackBarManager SnackBarProvider { get { return snackManager; } }

        public OverlayPanel()
        {
            this.DefaultStyleKey = typeof(OverlayPanel);
        }

        protected override void OnApplyTemplate()
        {
            PART_SnackBarGrid = GetTemplateChild(nameof(PART_SnackBarGrid)) as Grid;
            PART_OverlayGrid = GetTemplateChild(nameof(PART_OverlayGrid)) as Grid;
            PART_ContentPresenter = GetTemplateChild(nameof(PART_ContentPresenter)) as ContentPresenter;

            snackManager = new SnackBarManager(PART_SnackBarGrid);
            overlayManager = new OverlayPanelManager(this, PART_OverlayGrid);

            base.OnApplyTemplate();
        }

        internal void HideContent()
        {
            PART_ContentPresenter.Visibility = Visibility.Collapsed;
        }

        internal void ShowContent()
        {
            PART_ContentPresenter.Visibility = Visibility.Visible;
        }
    }
}

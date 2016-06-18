using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Kimono.Controls
{
    public sealed partial class GripperPanel : UserControl
    {
        public GripperPanel()
        {
            this.InitializeComponent();

            VisualStateManager.GoToState(this, InfoBoxCollapsedState.Name, true);
            IsExpanded = false;
        }

        public bool IsExpanded { get; private set; }

        private void GripperNub_Click(object sender, RoutedEventArgs e)
        {
            if (IsExpanded)
            {
                VisualStateManager.GoToState(this, InfoBoxCollapsedState.Name, true);
                IsExpanded = false;
            }
            else
            {
                VisualStateManager.GoToState(this, InfoBoxExpandedState.Name, true);
                IsExpanded = true;
            }
        }


        public static readonly DependencyProperty PanelContentProperty = DependencyProperty.Register("PanelContent", typeof(object), typeof(GripperPanel), new PropertyMetadata(null));
        public object PanelContent
        {
            get { return GetValue(PanelContentProperty); }
            set { SetValue(PanelContentProperty, value); }
        }
    }
}

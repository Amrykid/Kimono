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

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool),
            typeof(GripperPanel), new PropertyMetadata(false));
        public bool IsExpanded { get { return (bool)GetValue(IsExpandedProperty); } private set { SetValue(IsExpandedProperty, value); } }

        public static readonly DependencyProperty CollapsedHeightProperty = DependencyProperty.Register("CollapsedHeight", typeof(GridLength), typeof(GripperPanel),
            new PropertyMetadata(new GridLength(0.4, GridUnitType.Star), new PropertyChangedCallback((DependencyObject s, DependencyPropertyChangedEventArgs e) =>
            {
                if (((GridLength)e.NewValue).Value < ((GripperPanel)s).GripperNub.ActualHeight)
                    throw new ArgumentOutOfRangeException();

                if (!((GripperPanel)s).IsExpanded)
                    ((GripperPanel)s).InfoBoxRowDef.Height = (GridLength)e.NewValue;

            })));
        public GridLength CollapsedHeight { get { return (GridLength)GetValue(CollapsedHeightProperty); } set { SetValue(CollapsedHeightProperty, value); } }

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


        public static readonly DependencyProperty PanelContentProperty = DependencyProperty.Register("PanelContent", typeof(object),
            typeof(GripperPanel), new PropertyMetadata(null));
        public object PanelContent
        {
            get { return GetValue(PanelContentProperty); }
            set { SetValue(PanelContentProperty, value); }
        }

        private void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState == InfoBoxCollapsedState)
                InfoBoxRowDef.Height = CollapsedHeight;
        }
    }
}

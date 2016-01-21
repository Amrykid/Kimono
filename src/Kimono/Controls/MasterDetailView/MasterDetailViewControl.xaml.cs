using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class MasterDetailViewControl : UserControl
    {
        private volatile bool isInOnePaneMode = false;
        private double lastWindowWidth = 0;
        private double lastWindowHeight = 0;
        private SystemNavigationManager navigationManager = null;
        private volatile string currentState = "";

        public MasterDetailViewControl()
        {
            this.InitializeComponent();

            this.Loaded += MasterDetailViewControl_Loaded;
        }

        private void MasterDetailViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded += MasterDetailViewControl_Unloaded;

            this.DataContextChanged += MasterDetailViewControl_DataContextChanged;

            navigationManager = SystemNavigationManager.GetForCurrentView();
            //navigationManager.BackRequested += NavigationManager_BackRequested;

            Window.Current.SizeChanged += Current_SizeChanged;

            EvaluateLayout();
        }

        private void MasterDetailViewControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //may not be needed
            //masterViewContentControl.DataContext = this.DataContext;
            //detailViewContentControl.DataContext = this.DataContext;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            EvaluateLayout();
        }

        private void MasterDetailViewControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MasterDetailViewControl_Loaded;
            this.Unloaded -= MasterDetailViewControl_Unloaded;

            this.DataContextChanged -= MasterDetailViewControl_DataContextChanged;
            //navigationManager.BackRequested -= NavigationManager_BackRequested;
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void NavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (isInOnePaneMode)
            {
                if (PreviewItem != null)
                {
                    ShowMasterView();

                    e.Handled = true;
                }
            }
        }

        public void ShowMasterView()
        {
            if (NullifyPreviewItemWhenGoingToMasterView)
                PreviewItem = null;

            if (isInOnePaneMode)
            {
                //EvaluateLayout();

                lock (currentState)
                {
                    VisualStateManager.GoToState(this, "OnePaneMasterVisualState", true);
                    currentState = "OnePaneMasterVisualState";
                }

                if (BackButtonVisibilityHinted != null)
                    BackButtonVisibilityHinted(this, new BackButtonVisibilityHintedEventArgs(false));
            }
        }

        private void EvaluateLayout()
        {
            double width = Window.Current.Bounds.Width;
            double height = Window.Current.Bounds.Height;

            bool isOrientationChange = width == lastWindowHeight && height == lastWindowWidth;

            /* According to https://msdn.microsoft.com/en-us/library/windows/apps/dn997765.aspx - The recommend style is as follows:
             * 320 epx-719 epx (Available window width) = Stacked (Single pane shown at one time)
             * 720 epx or wider (Available window width) = Side-by-Side (Two panes shown at one time)
             */


            if (width >= 720)
            {
                isInOnePaneMode = false;

                lock (currentState)
                {
                    VisualStateManager.GoToState(this, "TwoPaneVisualState", true);

                    currentState = "TwoPaneVisualState";
                }

                if (BackButtonVisibilityHinted != null)
                    BackButtonVisibilityHinted(this, new BackButtonVisibilityHintedEventArgs(PreviewItem != null));
            }
            else
            {
                isInOnePaneMode = true;

                if (!isOrientationChange)
                {
                    var onePaneModeState = (PreviewItem != null ? "OnePaneDetailVisualState" : "OnePaneMasterVisualState");

                    lock (currentState)
                    {
                        VisualStateManager.GoToState(this, onePaneModeState, true);

                        currentState = onePaneModeState;
                    }

                    if (BackButtonVisibilityHinted != null)
                        BackButtonVisibilityHinted(this,
                            new BackButtonVisibilityHintedEventArgs(onePaneModeState == "OnePaneDetailVisualState"));

                    if (onePaneModeState == "OnePaneDetailVisualState")
                    {
                        //PART_detailViewScrollViewer.ScrollToVerticalOffset(0);
                    }
                }
            }


            lastWindowHeight = height;
            lastWindowWidth = width;
        }

        public static readonly DependencyProperty MasterViewPaneContentProperty = DependencyProperty.Register("MasterViewPaneContent", typeof(FrameworkElement),
            typeof(MasterDetailViewControl), new PropertyMetadata(null));

        public FrameworkElement MasterViewPaneContent
        {
            get { return (FrameworkElement)GetValue(MasterViewPaneContentProperty); }
            set { SetValue(MasterViewPaneContentProperty, value); }
        }

        public static readonly DependencyProperty DetailViewPaneContentProperty = DependencyProperty.Register("DetailViewPaneContent", typeof(FrameworkElement),
            typeof(MasterDetailViewControl), new PropertyMetadata(null));

        public FrameworkElement DetailViewPaneContent
        {
            get { return (FrameworkElement)GetValue(DetailViewPaneContentProperty); }
            set { SetValue(DetailViewPaneContentProperty, value); }
        }


        public static readonly DependencyProperty PreviewItemProperty = DependencyProperty.Register("PreviewItem", typeof(object),
            typeof(MasterDetailViewControl), new PropertyMetadata(null, new PropertyChangedCallback((control, args) =>
            {
                (control as MasterDetailViewControl).EvaluateLayout();
            })));

        /// <summary>
        /// The item that the preview pane is showing. This MUST be connected to a TwoWay binding.
        /// </summary>
        public object PreviewItem
        {
            get { return GetValue(PreviewItemProperty); }
            set { SetValue(PreviewItemProperty, value); }
        }


        public static readonly DependencyProperty NullifyPreviewItemWhenGoingToMasterViewProperty = DependencyProperty.Register("NullifyPreviewItemWhenGoingToMasterView", typeof(bool),
            typeof(MasterDetailViewControl), new PropertyMetadata(true, new PropertyChangedCallback((control, args) =>
            {
                (control as MasterDetailViewControl).EvaluateLayout();
            })));

        public bool NullifyPreviewItemWhenGoingToMasterView
        {
            get { return (bool)GetValue(NullifyPreviewItemWhenGoingToMasterViewProperty); }
            set { SetValue(NullifyPreviewItemWhenGoingToMasterViewProperty, value); }
        }

        public bool IsShowingDetailView()
        {
            if (currentState == "TwoPaneVisualState")
                return PreviewItem != null;
            else
                return currentState == "OnePaneDetailVisualState";
        }

        public event EventHandler<BackButtonVisibilityHintedEventArgs> BackButtonVisibilityHinted;
    }

    public class BackButtonVisibilityHintedEventArgs : EventArgs
    {
        internal BackButtonVisibilityHintedEventArgs(bool shouldBeVisible)
        {
            BackButtonShouldBeVisible = shouldBeVisible;
        }

        public bool BackButtonShouldBeVisible { get; private set; }
    }
}

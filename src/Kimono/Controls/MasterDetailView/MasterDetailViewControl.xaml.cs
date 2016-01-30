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
                ShowMasterView();

                e.Handled = true;

            }
        }

        public void ShowMasterView()
        {
            PreviewItem = null;

            if (isInOnePaneMode)
            {
                //EvaluateLayout();

                lock (currentState)
                {
                    VisualStateManager.GoToState(this, OnePaneMasterVisualState.Name, true);

                    if (currentState != OnePaneMasterVisualState.Name)
                    {
                        currentState = OnePaneMasterVisualState.Name;

                        if (MasterViewShown != null)
                            MasterViewShown(this, new MasterDetailViewControlViewShownEventArgs() { RequestedBy = ViewShownRequestedByType.User, ViewShown = currentState });
                    }
                }
            }
            else
            {
                currentState = TwoPaneVisualState.Name;

                if (MasterViewShown != null)
                    MasterViewShown(this, new MasterDetailViewControlViewShownEventArgs() { RequestedBy = ViewShownRequestedByType.User, ViewShown = currentState });
            }

            if (BackButtonVisibilityHinted != null)
                BackButtonVisibilityHinted(this, new MasterDetailViewControlBackButtonVisibilityHintedEventArgs(false));
        }

        public void ShowDetailView(object previewItem)
        {
            PreviewItem = previewItem;

            if (isInOnePaneMode)
            {
                //EvaluateLayout();

                lock (currentState)
                {
                    VisualStateManager.GoToState(this, OnePaneDetailVisualState.Name, true);

                    if (currentState != OnePaneDetailVisualState.Name)
                    {
                        currentState = OnePaneDetailVisualState.Name;

                        if (DetailViewShown != null)
                            DetailViewShown(this, new MasterDetailViewControlViewShownEventArgs() { RequestedBy = ViewShownRequestedByType.User, ViewShown = currentState });
                    }
                }
            }

            if (BackButtonVisibilityHinted != null)
                BackButtonVisibilityHinted(this, new MasterDetailViewControlBackButtonVisibilityHintedEventArgs(true));
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
                    VisualStateManager.GoToState(this, TwoPaneVisualState.Name, true);

                    if (currentState != TwoPaneVisualState.Name)
                    {
                        currentState = TwoPaneVisualState.Name;

                        if (MasterViewShown != null)
                            MasterViewShown(this, new MasterDetailViewControlViewShownEventArgs() { RequestedBy = ViewShownRequestedByType.Control, ViewShown = currentState });
                    }
                }

                if (BackButtonVisibilityHinted != null)
                    BackButtonVisibilityHinted(this, new MasterDetailViewControlBackButtonVisibilityHintedEventArgs(PreviewItem != null));
            }
            else
            {
                isInOnePaneMode = true;

                if (!isOrientationChange)
                {
                    var onePaneModeState = (PreviewItem != null ? OnePaneDetailVisualState.Name : OnePaneMasterVisualState.Name);

                    lock (currentState)
                    {
                        VisualStateManager.GoToState(this, onePaneModeState, true);

                        if (currentState != OnePaneMasterVisualState.Name && onePaneModeState == OnePaneMasterVisualState.Name)
                        {
                            if (MasterViewShown != null)
                                MasterViewShown(this, new MasterDetailViewControlViewShownEventArgs() { RequestedBy = ViewShownRequestedByType.Control, ViewShown = onePaneModeState });
                        }
                        else if (currentState != OnePaneDetailVisualState.Name && onePaneModeState == OnePaneDetailVisualState.Name)
                        {
                            if (DetailViewShown != null)
                                DetailViewShown(this, new MasterDetailViewControlViewShownEventArgs() { RequestedBy = ViewShownRequestedByType.Control, ViewShown = onePaneModeState });
                        }
                            
                        currentState = onePaneModeState;
                    }

                    if (BackButtonVisibilityHinted != null)
                        BackButtonVisibilityHinted(this,
                            new MasterDetailViewControlBackButtonVisibilityHintedEventArgs(onePaneModeState == OnePaneDetailVisualState.Name));

                    if (onePaneModeState == OnePaneDetailVisualState.Name)
                    {
                        //PART_detailViewScrollViewer.ScrollToVerticalOffset(0);
                    }
                }
            }


            lastWindowHeight = height;
            lastWindowWidth = width;

            this.InvalidateMeasure();
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
            typeof(MasterDetailViewControl), new PropertyMetadata(null));

        public object PreviewItem
        {
            get { return GetValue(PreviewItemProperty); }
            private set { SetValue(PreviewItemProperty, value); }
        }

        public bool IsShowingDetailView()
        {
            if (currentState == "TwoPaneVisualState")
                return true;
            else
                return currentState == OnePaneDetailVisualState.Name;
        }

        public event EventHandler<MasterDetailViewControlBackButtonVisibilityHintedEventArgs> BackButtonVisibilityHinted;

        public event EventHandler<MasterDetailViewControlViewShownEventArgs> MasterViewShown;
        public event EventHandler<MasterDetailViewControlViewShownEventArgs> DetailViewShown;
    }

    public class MasterDetailViewControlBackButtonVisibilityHintedEventArgs : EventArgs
    {
        internal MasterDetailViewControlBackButtonVisibilityHintedEventArgs(bool shouldBeVisible)
        {
            BackButtonShouldBeVisible = shouldBeVisible;
        }

        public bool BackButtonShouldBeVisible { get; private set; }
    }

    public class MasterDetailViewControlViewShownEventArgs: EventArgs
    {
        public ViewShownRequestedByType RequestedBy { get; internal set; }
        public string ViewShown { get; internal set; }
    }

    public enum ViewShownRequestedByType
    {
        Control = 0,
        User = 1
    }
}

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
    [Windows.UI.Xaml.Markup.ContentProperty(Name = "LoadedContent")]
    public sealed partial class LoadingContentContainer : UserControl
    {
        public LoadingContentContainer()
        {
            this.InitializeComponent();

            VisualStateManager.GoToState(this, "UnloadedState", true);
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool),
            typeof(LoadingContentContainer), new PropertyMetadata(false, new PropertyChangedCallback((control, args) =>
            {
                var container = control as LoadingContentContainer;
                var args2 = args as DependencyPropertyChangedEventArgs;

                if ((bool)args2.NewValue)
                {
                    container.GoToLoadingState();
                }
                else
                {
                    container.GoToLoadedState();
                }
            })));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public void GoToLoadingState()
        {
            VisualStateManager.GoToState(this, "LoadingState", true);
        }

        public void GoToLoadedState()
        {
            VisualStateManager.GoToState(this, "LoadedState", true);
        }

        public static readonly DependencyProperty LoadedContentProperty = DependencyProperty.Register("LoadedContent", typeof(FrameworkElement),
            typeof(LoadingContentContainer), new PropertyMetadata(null));

        public FrameworkElement LoadedContent
        {
            get { return (FrameworkElement)GetValue(LoadedContentProperty); }
            set { SetValue(LoadedContentProperty, value); }
        }
    }
}

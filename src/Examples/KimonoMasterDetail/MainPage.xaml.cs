using System;
using System.Collections.Generic;
using System.ComponentModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KimonoMasterDetail
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            this.DataContext = new SomeViewModel();
        }

        public class SomeViewModel: INotifyPropertyChanged
        {
            public string[] DataItems
            {
                get { return new string[] { "Test", "Test 2", "Test 3", "Test 4" }; }
            }

            private object _selectedItem = null;
            public object SelectedItem
            {
                get { return _selectedItem; }
                set
                {
                    _selectedItem = value;

                    if (PropertyChanged != null)
                        PropertyChanged(null, new PropertyChangedEventArgs("SelectedItem"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}

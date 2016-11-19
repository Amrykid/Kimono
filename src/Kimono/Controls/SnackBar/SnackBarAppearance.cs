﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace Kimono.Controls.SnackBar
{
    public static class SnackBarAppearance
    {
        public static double Opacity { get; set; } = 0.8;
        public static double MessageFontSize { get; set; } = 12;
        public static Transition Transition { get; set; } = new AddDeleteThemeTransition();
    }
}

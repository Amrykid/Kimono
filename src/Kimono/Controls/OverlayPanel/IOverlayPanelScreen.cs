using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimono.Controls
{
    public interface IOverlayPanelScreen
    {
        void OnShown(OverlayPanel parentOverlay);
        void OnClosed();
    }
}

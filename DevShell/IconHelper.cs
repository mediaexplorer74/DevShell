using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;

namespace DevShell
{
    public static class IconHelper
    {
        public static ImageSource GetIconImageSource(Process p)
        {
            return Icon.FromHandle(p.MainWindowHandle).ToImageSource();
        }
    }
}
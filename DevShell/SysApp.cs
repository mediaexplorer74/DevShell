using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;

namespace DevShell
{
    public class SysApp
    {
        public string DisplayName { get; set; }

        public string IconPath { get; set; }

        public bool IsIconVisible => Icon != null;

        public Icon Icon { get; set; }
        public ImageSource ImageSource { get; set; }

        public Process Process { get; set; }
    }
}
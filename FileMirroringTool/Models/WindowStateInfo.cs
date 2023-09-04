using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace FileMirroringTool.Models
{
    public class WindowStateInfo
    {
        internal WindowStateInfo() { }
        internal WindowStateInfo(Window window)
        {
            Top = window.Top;
            Left = window.Left;
            Width = window.Width;
            Height = window.Height;
            HasSetting = true;
        }
        public bool HasSetting { get; set; } = false;
        public double Top { get; set; } = 0;
        public double Left { get; set; } = 0;
        public double Width { get; set; } = 100;
        public double Height { get; set; } = 100;

        internal bool IsInScrreen()
        {
            foreach (var screen in Screen.AllScreens)
            {
                var screenBounds = screen.Bounds;
                var windowBounds = new Rectangle((int)Left, (int)Top, (int)Width, (int)Height);
                if (screenBounds.Contains(windowBounds)) return true;
            }
            return false;
        }
    }
}

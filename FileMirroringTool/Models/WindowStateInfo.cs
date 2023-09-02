using System.Windows;

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
    }
}

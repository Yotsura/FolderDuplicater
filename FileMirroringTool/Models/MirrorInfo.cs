using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMirroringTool.Models
{
    public class MirrorInfo
    {
        public int Idx { get; set; } = -1;
        public bool IsChecked { get; set; } = true;
        public string OrigPath { get; set; } = string.Empty;
        public string DestPath { get; set; } = string.Empty;
    }
}

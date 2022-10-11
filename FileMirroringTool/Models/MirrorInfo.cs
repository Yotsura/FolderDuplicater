using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMirroringTool.Models
{
    public class MirrorInfo
    {
        public int Idx { get; set; }
        public bool IsChecked { get; set; }
        public string OrigPath { get; set; }
        public string DestPath { get; set; }
    }
}

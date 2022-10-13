using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMirror.Models
{
    internal class MirrorInfo
    {
        internal bool IsEnabled { get; set; }
        internal string OrigPath { get; set; }
        internal string DestinationPath { get; set; }
    }
}

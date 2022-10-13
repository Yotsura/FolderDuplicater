using FileMirroringTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMirroringTool
{
    internal sealed partial class Settings
    {
        [System.Configuration.UserScopedSetting()]
        [System.Diagnostics.DebuggerNonUserCode()]
        public List<MirrorInfo> MirrorList
        {
            get => ((List<MirrorInfo>)this[nameof(MirrorList)]) ?? new List<MirrorInfo>();
            set => this[nameof(MirrorList)] = value;
        }
    }
}

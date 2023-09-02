using FileMirroringTool.Models;
using System.Collections.Generic;

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

        [System.Configuration.UserScopedSetting()]
        [System.Diagnostics.DebuggerNonUserCode()]
        public List<string> CheckedFilePaths
        {
            get => ((List<string>)this[nameof(CheckedFilePaths)]) ?? new List<string>();
            set => this[nameof(CheckedFilePaths)] = value;
        }

        [System.Configuration.UserScopedSetting()]
        [System.Diagnostics.DebuggerNonUserCode()]
        public WindowStateInfo MainWindowStateInfo
        {
            get => (WindowStateInfo)this[nameof(MainWindowStateInfo)] ?? new WindowStateInfo();
            set => this[nameof(MainWindowStateInfo)] = value;
        }

        [System.Configuration.UserScopedSetting()]
        [System.Diagnostics.DebuggerNonUserCode()]
        public WindowStateInfo SubWindowStateInfo
        {
            get => (WindowStateInfo)this[nameof(SubWindowStateInfo)] ?? new WindowStateInfo();
            set => this[nameof(SubWindowStateInfo)] = value;
        }
    }
}

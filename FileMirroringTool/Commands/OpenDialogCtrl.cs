using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileMirroringTool.Commands
{
    internal class OpenDialogCtrl : ICommand
    {
        readonly MainWindowViewModel _mwvm;
        public OpenDialogCtrl(MainWindowViewModel mwvm)
        {
            _mwvm = mwvm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var path = FolderDialog();
            if (string.IsNullOrEmpty(path)) return;
            switch (parameter.ToString())
            {
                case "orig":
                    _mwvm.OrigPath = path;
                    break;
                case "dest":
                    _mwvm.DestPath += (string.IsNullOrEmpty(_mwvm.DestPath) ? string.Empty : "\n") + path;
                    break;
            }
        }

        public static string FolderDialog()
        {
            var dlg = new CommonOpenFileDialog("IFwwwフォルダ選択")
            {
                IsFolderPicker = true// フォルダ選択モード。
            };
            var ret = dlg.ShowDialog();
            return ret == CommonFileDialogResult.Ok ? dlg.FileName : string.Empty;
        }
    }
}

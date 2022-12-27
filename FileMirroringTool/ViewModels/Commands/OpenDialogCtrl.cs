using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows.Input;

namespace FileMirroringTool.ViewModels.Commands
{
    internal class OpenDialogCtrl : ICommand
    {
        readonly MainWindowViewModel _mwvm;
        public OpenDialogCtrl(MainWindowViewModel mwvm)
        {
            _mwvm = mwvm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            switch (parameter.ToString())
            {
                case "orig":
                case "dest":
                    return true;
                case "backup":
                    var selectedOrigPath = _mwvm.SelectedMirrorInfo?.OrigPath ?? string.Empty;
                    return !string.IsNullOrEmpty(selectedOrigPath)
                        && new System.IO.DirectoryInfo(selectedOrigPath).Exists;
                    //return true;
                default: return false;
            }
        }

        public void Execute(object parameter)
        {
            var para = parameter.ToString();
            switch (para)
            {
                case "orig":
                    _mwvm.OrigPath = FolderDialog();
                    break;
                case "dest":
                    _mwvm.DestPath += (string.IsNullOrEmpty(_mwvm.DestPath) ? string.Empty : "\n") + FolderDialog();
                    break;
                case "backup":
                    _mwvm.SearchFile = FileDialog(_mwvm.OrigPath);
                    break;
            }
        }

        public static string FileDialog(string iniPath)
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = iniPath;
            dialog.Filter = "すべてのファイル(*.*)|*.*";
            dialog.FilterIndex = 2;
            return dialog.ShowDialog() == true ? dialog.FileName : string.Empty;
        }

        public static string FolderDialog()
        {
            var dlg = new CommonOpenFileDialog("フォルダ選択")
            {
                IsFolderPicker = true// フォルダ選択モード。
            };
            return dlg.ShowDialog() == CommonFileDialogResult.Ok ? dlg.FileName : string.Empty;
        }
    }
}

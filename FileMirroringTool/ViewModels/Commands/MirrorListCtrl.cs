using FileMirroringTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FileMirroringTool.ViewModels.Commands
{
    internal class MirrorListCtrl : ICommand
    {
        readonly MainWindowViewModel _mwvm;
        public MirrorListCtrl(MainWindowViewModel mwvm)
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
            if (string.IsNullOrEmpty(_mwvm.OrigPath) ||
                string.IsNullOrEmpty(_mwvm.DestPath))
                return false;

            switch (parameter.ToString())
            {
                case "add":
                    return true;
                    //return _mwvm.SelectedMirrorInfo == null;
                case "upd":
                case "del":
                    return _mwvm.SelectedMirrorInfo != null;
                default:
                    return false;
            }
        }

        public void Execute(object parameter)
        {
            var nextID = (_mwvm.MirrorList.OrderByDescending(x => x.ID).FirstOrDefault()?.ID ?? 0) + 1;
            var inputdata = new MirrorInfo
            {
                ID = _mwvm.SelectedMirrorInfo?.ID ?? nextID,
                Sort = int.TryParse(_mwvm.Sort, out var snum) ? snum : 0,
                NeedBackup = _mwvm.NeedBackup,
                SkipExclamation = _mwvm.SkipExclamation,
                EncryptMode = _mwvm.EncryptMode,

                OrigPath = _mwvm.OrigPath,
                DestPathsStr = _mwvm.DestPath,
            };
            //add updの場合、各パスが存在するかチェック
            if (parameter.ToString() == "add" || parameter.ToString() == "upd")
            {
                var errmsg = new List<string>();
                var sourceDir = new DirectoryInfo(inputdata.OrigPath);
                if (!sourceDir.Exists) errmsg.Add("Sourceのフォルダが見つかりません。");
                var sourceTopDirs = sourceDir.EnumerateDirectories();
                var sourceTopFiles = sourceDir.EnumerateFiles();
                inputdata.ExistDestPathsList.ForEach(dest =>
                {
                    var destDir = new DirectoryInfo(dest);
                    var destTopDirs = destDir.EnumerateDirectories();
                    var destTopFiles = destDir.EnumerateFiles();
                    if (!destTopDirs.Any() && !destTopFiles.Any()) return;
                    //origとdestを比較し、一番上の階層でフォルダ名/ファイル名が全く一致しない場合、設定ミスの可能性があるので警告
                    //sourceにないフォルダ/ファイルがdestにある場合、かつsourceにあるもの
                });
                if (errmsg.Any())
                    MessageBox.Show(string.Join("\r\n", errmsg)+ "\r\n各パスをご確認ください。設定を追加/更新しますか？", "警告",
                        MessageBoxButton.OKCancel, MessageBoxImage.Information);
            }

            var targetItem = _mwvm.MirrorList.FirstOrDefault(x => x.ID == inputdata.ID);
            switch (parameter.ToString())
            {
                case "add":
                    inputdata.ID = nextID;
                    _mwvm.MirrorList.Add(inputdata);
                    _mwvm.Sort = string.Empty;
                    _mwvm.EncryptMode = 0;
                    _mwvm.OrigPath = string.Empty;
                    _mwvm.DestPath = string.Empty;
                    break;
                case "upd":
                    _mwvm.SelectedMirrorInfo = null;
                    _mwvm.MirrorList.Remove(targetItem);
                    _mwvm.MirrorList.Add(inputdata);
                    break;
                case "del":
                    _mwvm.MirrorList.Remove(targetItem);
                    break;
            }
        }
    }
}

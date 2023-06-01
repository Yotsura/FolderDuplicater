using FileMirroringTool.Models;
using System;
using System.Linq;
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
            if (string.IsNullOrEmpty(_mwvm.SelectedMirrorInfo.OrigPath) ||
                string.IsNullOrEmpty(_mwvm.SelectedMirrorInfo.DestPathsStr))
                return false;

            switch (parameter.ToString())
            {
                case "add":
                    return true;
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
            var recordID = _mwvm.SelectedMirrorInfo?.ID ?? nextID;
            var inputdata = _mwvm.SelectedMirrorInfo;
            var targetItem = _mwvm.MirrorList.FirstOrDefault(x => x.ID == (_mwvm.SelectedMirrorInfo?.ID ?? nextID));
            switch (parameter.ToString())
            {
                case "add":
                    inputdata.ID = nextID;
                    _mwvm.MirrorList.Add(inputdata);
                    break;
                case "upd":
                    _mwvm.MirrorList.Remove(targetItem);
                    _mwvm.MirrorList.Add(inputdata);
                    break;
                case "del":
                    _mwvm.MirrorList.Remove(targetItem);
                    break;
            }
            _mwvm.SelectedMirrorInfo = new MirrorInfo();
        }
    }
}

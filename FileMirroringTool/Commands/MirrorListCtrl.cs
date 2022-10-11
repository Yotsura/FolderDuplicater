using FileMirroringTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileMirroringTool.Commands
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
                    return _mwvm.SelectedMirrorInfo == null;
                case "upd":
                case "del":
                    return _mwvm.SelectedMirrorInfo != null;
                default:
                    return false;
            }
        }

        public void Execute(object parameter)
        {
            var idx = _mwvm.MirrorList.OrderByDescending(x => x.Idx).FirstOrDefault()?.Idx ?? 0;

            var inputdata = new MirrorInfo
            {
                Idx = _mwvm.SelectedMirrorInfo?.Idx ?? (idx + 1),
                OrigPath = _mwvm.OrigPath,
                DestPath = _mwvm.DestPath,
            };
            var targetItem = _mwvm.MirrorList.FirstOrDefault(x => x.Idx == inputdata.Idx);
            switch (parameter.ToString())
            {
                case "add":
                    _mwvm.MirrorList.Add(inputdata);
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

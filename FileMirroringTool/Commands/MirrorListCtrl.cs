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
            switch (parameter.ToString())
            {
                case "add":
                    return true;
                case "upd":
                    return true;
                case "del":
                    return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            var inputdata = new MirrorInfo
            {
                OrigPath = _mwvm.OrigPath,
                DestPath = _mwvm.DestPath,
            };
            switch (parameter.ToString())
            {
                case "add":
                    _mwvm.MirrorList.Add(inputdata);
                    break;
                case "upd":
                    //_mwvm.MirrorList.First(x => x == inputdata) = inputdata;
                    break;
                case "del":
                    _mwvm.MirrorList.Remove(inputdata);
                    break;
            }
        }
    }
}

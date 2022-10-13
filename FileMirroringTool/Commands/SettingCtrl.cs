using FileMirroringTool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileMirroringTool.Commands
{
    internal class SettingCtrl : ICommand
    {
        readonly MainWindowViewModel _mwvm;
        public SettingCtrl(MainWindowViewModel mwvm)
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
                case "save":
                    var list1 = _mwvm.MirrorList.OrderBy(x=>x.ID).ToList();
                    var list2 = Settings.Default.MirrorList.OrderBy(x=>x.ID).ToList();
                    var result = !list1.SequenceEqual(list2);
                    return result;
                case "reload":
                    return true;
                default:
                    return false;
            }
        }

        public void Execute(object parameter)
        {

            switch (parameter.ToString())
            {
                case "save":
                    Settings.Default.MirrorList = _mwvm.MirrorList.ToList();
                    Settings.Default.Save();
                    System.Windows.MessageBox.Show("設定を保存しました。");
                    break;
                case "reload":
                    _mwvm.MirrorList = new ObservableCollection<MirrorInfo>(Settings.Default.MirrorList);
                    _mwvm.SelectedMirrorInfo = null;
                    System.Windows.MessageBox.Show("設定を再度読み込みました。");
                    break;
            }
        }
    }
}

using FileMirroringTool.ViewModels;
using System;
using System.Linq;
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
                    while (_mwvm.MirrorList.Count > 0)
                        _mwvm.MirrorList.RemoveAt(0);
                    Settings.Default.MirrorList.ForEach(mirror => _mwvm.MirrorList.Add(mirror));
                    _mwvm.SelectedMirrorInfo = null;
                    System.Windows.MessageBox.Show("設定を再度読み込みました。");
                    break;
            }
        }
    }
}

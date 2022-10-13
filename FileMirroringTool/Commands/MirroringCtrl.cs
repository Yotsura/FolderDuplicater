using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileMirroringTool.Commands
{
    internal class MirroringCtrl : ICommand
    {
        readonly MainWindowViewModel _mwvm;
        public MirroringCtrl(MainWindowViewModel mwvm)
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
            return _mwvm.MirrorList.Count() > 0;
        }

        public void Execute(object parameter)
        {
            //Progressウィンドウ開く
            CancellationTokenSource cancelToken = new CancellationTokenSource();
            ProgressDialog pd = new ProgressDialog(_mwvm, () =>
            {
                _mwvm.MirrorList.Where(x => x.IsChecked)
                    .OrderBy(x => x.SortPara)
                    .ToList().ForEach(mirror =>
                    {
                        _mwvm.ResetPrgStat();
                        mirror.MirroringInvoke(_mwvm);
                    });
            }, cancelToken);

            pd.ShowDialog();
            if (pd.IsCanceled)
                System.Windows.MessageBox.Show("ミラーリングが停止されました。");
            else
                System.Windows.MessageBox.Show("ミラーリングが完了しました。");
        }
    }
}

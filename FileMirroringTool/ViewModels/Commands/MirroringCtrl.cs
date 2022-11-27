using FileMirroringTool.Views;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace FileMirroringTool.ViewModels.Commands
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
            _mwvm.DelCnt = 0;
            _mwvm.AddCnt = 0;
            _mwvm.UpdCnt = 0;
            _mwvm.FileCnt_Checked = 0;

            //Progressウィンドウ開く
            CancellationTokenSource cancelToken = new CancellationTokenSource();
            ProgressDialog pd = new ProgressDialog(_mwvm, () =>
            {
                _mwvm.MirrorList.Where(x => x.IsChecked && x.CanExecuteMirroring)
                    .OrderBy(x => x.SortPara)
                    .ToList().ForEach(mirror =>
                    {
                        _mwvm.ResetPrgStat();
                        mirror.MirroringInvoke(_mwvm);
                    });
            }, cancelToken);

            pd.ShowDialog();
            var result = $"\r\n追加ファイル：{_mwvm.AddCnt}\r\n" +
                $"更新ファイル：{_mwvm.UpdCnt}\r\n" +
                $"削除ファイル：{_mwvm.DelCnt}\r\n";

            if (pd.IsCanceled || !pd.IsCompleted)
                System.Windows.MessageBox.Show("ミラーリングが停止されました。" + result);
            else
                System.Windows.MessageBox.Show("ミラーリングが完了しました。" + result);
        }
    }
}

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
            _mwvm.FileCnt_Checked = 0;
            Settings.Default.MirrorList = _mwvm.MirrorList.ToList();
            Settings.Default.Save();

            var result = string.Empty;
            //Progressウィンドウ開く
            var cancelTokenSource = new CancellationTokenSource();
            var cancelToken = cancelTokenSource.Token;
            ProgressDialog pd = new ProgressDialog(_mwvm, () =>
            {
                _mwvm.MirrorList.Where(x => x.IsChecked && x.CanExecuteMirroring)
                    .OrderBy(x => x.SortPara)
                    .ToList().ForEach(mirror =>
                    {
                        _mwvm.ResetPrgStat();
                        mirror.MirroringInvoke(_mwvm, cancelToken);

                        result += $"\r\n【ID：{mirror.ID}（backup：{mirror.BackupSpans}）】"
                            + mirror.FileCounter.CntInfoStr;
                        if (cancelToken.IsCancellationRequested) return;
                    });
            }, cancelTokenSource);
            pd.ShowDialog();

            System.Windows.MessageBox.Show("ミラーリングが" +
                (pd.IsCompleted ? "完了しました。" : "中止されました。") +
                result);
            cancelTokenSource.Dispose();
        }
    }
}

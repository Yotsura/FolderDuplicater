using FileMirroringTool.Models;
using FileMirroringTool.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
namespace FileMirroringTool.ViewModels.Commands
{
    internal class MirroringCtrl : ICommand
    {
        readonly MainWindowViewModel _mwvm;
        List<MirrorInfo> MirrorList
            => _mwvm.MirrorList.Where(x => x.IsChecked && x.CanExecuteMirroring)
                    .OrderBy(x => x.SortPara).ToList();
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

            _mwvm.SearchFile = string.Empty;
            if (parameter.ToString() == "auto")
                AutoMirror();
            else
                ManualMirror();
        }

        void ManualMirror()
        {
            var result = string.Empty;
            var cancelTokenSource = new CancellationTokenSource();
            var cancelToken = cancelTokenSource.Token;
            var pd = new ProgressDialog(_mwvm, () =>
            {
                MirrorList.ForEach(mirror =>
                {
                    _mwvm.ResetPrgStat();
                    mirror.MirroringInvoke(_mwvm, cancelToken);
                    result += $"\r\n【ID：{mirror.ID}（backup：{(mirror.NeedBackup ? "on" : "off")}）】"
                        + mirror.FileCounter.CntInfoStr;
                    _mwvm.ResetPrgStat();

                    if (cancelToken.IsCancellationRequested) return;
                });
            }, cancelTokenSource, false);
            pd.ShowDialog();

            System.Windows.MessageBox.Show("ミラーリングが" +
                (pd.IsCompleted ? "完了しました。" : "中止されました。") +
                result);
            cancelTokenSource.Dispose();

        }

        void AutoMirror()
        {
            if (_mwvm.AutoInterval <= 0) _mwvm.AutoInterval = 1;
            Settings.Default.AutomationIntervalStr = _mwvm.AutoIntervalStr;
            Settings.Default.Save();

            var cancelTokenSource = new CancellationTokenSource();
            var cancelToken = cancelTokenSource.Token;
            var runCnt = 0;
            var lastRunTime = DateTime.Now;
            var pd = new ProgressDialog(_mwvm, () =>
            {
                if (cancelToken.IsCancellationRequested) return;
                lastRunTime = DateTime.Now;
                runCnt++;
                MirrorList.ForEach(mirror =>
                {
                    _mwvm.ResetPrgStat();
                    if (cancelToken.IsCancellationRequested) return;
                    mirror.MirroringInvoke(_mwvm, cancelToken);
                    if (cancelToken.IsCancellationRequested) return;
                });
                _mwvm.ResetPrgStat();
                _mwvm.PrgTitle = $"＜待機中＞" +
                $"ミラーリング周期：{_mwvm.AutoInterval}hours　" +
                $"次回実行時刻：{lastRunTime.AddHours(_mwvm.AutoInterval).ToString("HH:mm:ss")}";
            }, cancelTokenSource, true);

            pd.Closed += (e, s) =>
            {
                System.Windows.MessageBox.Show("自動ミラーリングが停止されました。"
                    + $"\r\n最後の実行：{(runCnt > 0 ? lastRunTime.ToString("HH:mm:ss") : string.Empty)}");
                cancelTokenSource.Dispose();
            };
            pd.ShowDialog();
        }
    }
}

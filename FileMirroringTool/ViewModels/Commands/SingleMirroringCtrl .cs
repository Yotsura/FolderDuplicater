using FileMirroringTool.Models;
using FileMirroringTool.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
namespace FileMirroringTool.ViewModels.Commands
{
    internal class SingleMirroringCtrl : ICommand
    {
        readonly MainWindowViewModel _mwvm;
        public SingleMirroringCtrl(MainWindowViewModel mwvm)
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
            return _mwvm.SelectedMirrorInfo != null;
        }

        public void Execute(object param)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Settings.Default.MirrorList = _mwvm.MirrorList.ToList();
            Settings.Default.Save();

            var cancelTokenSource = new CancellationTokenSource();
            var cancelToken = cancelTokenSource.Token;
            var mirror = _mwvm.SelectedMirrorInfo;
            var pd = new ProgressDialog(_mwvm, () =>
            {
                _mwvm.ResetPrgStat();
                mirror.MirroringInvoke(_mwvm, cancelToken);
                _mwvm.ResetPrgStat();

                if (cancelToken.IsCancellationRequested) return;
            }, cancelTokenSource, false);
            pd.ShowDialog();

            sw.Stop();

            var result = $"【ID：{mirror.ID}（backup：{(mirror.NeedBackup ? "on" : "off")}）】{mirror.FileCounter.CntInfoStr}";
            System.Windows.MessageBox.Show($"ミラーリングが{(pd.IsCompleted ? "完了しました。" : "中止されました。")}\r\n{result}\r\n実行時間：{sw.Elapsed}");
            cancelTokenSource.Dispose();
        }
    }
}

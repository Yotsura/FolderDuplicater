using FileMirroringTool.Utils;
using FileMirroringTool.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace FileMirroringTool.ViewModels.Commands
{
    public class FixExtensionsCtrl : ICommand
    {
        readonly MainWindowViewModel _mwvm;
        public FixExtensionsCtrl(MainWindowViewModel mwvm)
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
            return !string.IsNullOrEmpty(_mwvm.OrigPath);
        }

        public void Execute(object parameter)
        {
            if (MessageBox.Show($"Sourceフォルダ内の画像ファイルについて、" +
                $"\r\n先頭バイトから拡張子を解析し正常な拡張子が設定されているか確認します。" +
                $"\r\n一致しない場合は正常な拡張子に修正します。" +
                $"\r\n対象フォルダ：" +
                $"\r\n{_mwvm.OrigPath}", "実行結果" ,MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;
            _mwvm.ResetPrgStat();

            var cancelTokenSource = new CancellationTokenSource();
            var cancelToken = cancelTokenSource.Token;
            cancelToken.ThrowIfCancellationRequested();

            var fixeImgs = new List<string>();
            var pd = new ProgressDialog(_mwvm, () =>
            {
                try
                {
                    _mwvm.ResetPrgStat();
                    var target = Directory.EnumerateFiles(_mwvm.OrigPath, "*", SearchOption.AllDirectories).AsParallel().Where(x => x.IsImgFile());
                    _mwvm.FileCnt_Target = target.Count();
                    target.ForAll(path =>
                        {
                            if (cancelToken.IsCancellationRequested) return;
                            if (path.ShouldFixImgFileExtension(out var fixedPath))
                            {
                                File.Move(path, fixedPath);
                                fixeImgs.Add(fixedPath);
                            }
                            _mwvm.FileCnt_Checked++;
                        });
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Print($"Exception: {e.GetType().Name}");
                }
            }, cancelTokenSource, false);
            pd.ShowDialog();

            MessageBox.Show($"拡張子チェックが{(pd.IsCompleted ? "完了しました。" : "中止されました。")}\r\n修正済件数：{fixeImgs.Count}件","実行結果");
            cancelTokenSource.Dispose();
        }
    }
}

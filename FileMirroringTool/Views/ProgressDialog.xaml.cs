using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FileMirroringTool.Views
{
    /// <summary>
    /// ProgressDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog(object context, Action action, CancellationTokenSource cancelToken,bool isAuto)
        {
            InitializeComponent();
            DataContext = context;
            _action = action;
            _cancelToken = cancelToken;
            if (isAuto)
                _worker.DoWork += DoWork_Auto;
            else
                _worker.DoWork += DoWork;
            if (!isAuto)
                _worker.RunWorkerCompleted += RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private readonly BackgroundWorker _worker = new BackgroundWorker();

        private readonly Action _action;

        private readonly CancellationTokenSource _cancelToken;
        public bool IsCompleted { get; set; } = false;

        private System.Timers.Timer _timer
            = new System.Timers.Timer(new TimeSpan(1, 0, 0).TotalMilliseconds);
        private void DoWork_Auto(object sender, DoWorkEventArgs e)
        {
            _timer.Elapsed += (te, s) => DoAction();
            DoAction();//一度実行してからタイマースタート
            _timer.Start();
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            DoAction();
        }

        private void DoAction()
        {
            if (_action == null) return;
            Task task = Task.Factory.StartNew((obj) =>
            {
                _action.Invoke();
            }, _cancelToken);
            task.Wait();
        }
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsCompleted = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            _timer.Stop();
            if (!IsCompleted) _cancelToken.Cancel();
        }
    }
}

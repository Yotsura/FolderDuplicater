using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FileMirroringTool.Views
{
    /// <summary>
    /// ProgressDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog(object context, Action action, CancellationTokenSource cancelToken)
        {
            InitializeComponent();
            DataContext = context;
            _action = action;
            _cancelToken = cancelToken;
            _worker.DoWork += DoWork;
            _worker.RunWorkerCompleted += RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private readonly BackgroundWorker _worker = new BackgroundWorker();

        private readonly Action _action;

        private readonly CancellationTokenSource _cancelToken;
        public bool IsCanceled { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        private void DoWork(object sender, DoWorkEventArgs e)
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
            _cancelToken.Cancel();
            IsCanceled = true;
        }
    }
}

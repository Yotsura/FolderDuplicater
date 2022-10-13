using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileMirroringTool
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
            this.action = action;
            this.cancelToken = cancelToken;
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private BackgroundWorker worker = new BackgroundWorker();

        private Action action;

        private CancellationTokenSource cancelToken;

        private bool isCanceled = false;
        public bool IsCanceled
        {
            get
            {
                return isCanceled;
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (action == null)
            {
                return;
            }
            Task task = Task.Factory.StartNew((obj) =>
            {
                action.Invoke();
            }, cancelToken);

            task.Wait();
        }
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            cancelToken.Cancel();
            isCanceled = true;
        }
    }
}

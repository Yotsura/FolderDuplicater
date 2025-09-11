using System.Threading;
using System.Windows;

namespace FileMirroringTool
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex;

        public App()
        {
            bool createdNew;
            mutex = new Mutex(true, "Dialy", out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("アプリケーションは既に実行中です。");
                Shutdown();
                return;
            }

            Exit += OnAppExit;
        }

        private void OnAppExit(object sender, ExitEventArgs e)
        {
            mutex?.ReleaseMutex();
            mutex = null;
        }
    }
}

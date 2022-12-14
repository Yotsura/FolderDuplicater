using System.Windows;
using System.Windows.Input;

namespace FileMirroringTool.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new ViewModels.MainWindowViewModel();
            InitializeComponent();
            MainDataGrid.UnselectAll();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (MainDataGrid.SelectedIndex > -1)
                MainDataGrid.UnselectAll();
        }
    }
}

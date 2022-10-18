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
            InitializeComponent();
            DataContext = new ViewModels.MainWindowViewModel();
            MainDataGrid.UnselectAll();
        }

        private void DataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (MainDataGrid.SelectedIndex > -1)
                MainDataGrid.UnselectAll();
        }
    }
}

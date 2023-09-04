using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
            LoadWindowStateInfo();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (MainDataGrid.SelectedIndex < 0) return;
            //DataGrid内のアイテムがクリックされた場合はreturn
            var dgRowCtrl = (DataGridRow)MainDataGrid.ItemContainerGenerator.ContainerFromItem(MainDataGrid.SelectedItem);
            if (dgRowCtrl?.InputHitTest(e.GetPosition(dgRowCtrl)) != null) return;
            MainDataGrid.UnselectAll();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //数値のみ許可
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
        private void TextBox_PreviewTextInput2(object sender, TextCompositionEventArgs e)
        {
            //数値とカンマを許可
            e.Handled = new Regex(@"([^0-9|\,|\.])+").IsMatch(e.Text);
        }
        private void TextBox_PreviewTextInput3(object sender, TextCompositionEventArgs e)
        {
            //数値と小数を許可
            e.Handled = new Regex(@"([^0-9|\.])+").IsMatch(e.Text);
        }

        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 貼り付けを禁止します。
            e.Handled = e.Command == ApplicationCommands.Paste;
        }

        private void CheckBox_CheckChangeed(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
        }

        private void LoadWindowStateInfo()
        {
            if (!Settings.Default.MainWindowStateInfo.HasSetting) return;

            Width = Settings.Default.MainWindowStateInfo.Width;
            Height = Settings.Default.MainWindowStateInfo.Height;
            var isinscreen = Settings.Default.MainWindowStateInfo.IsInScrreen();
            Left = isinscreen ? Settings.Default.MainWindowStateInfo.Left : 10;
            Top = isinscreen ? Settings.Default.MainWindowStateInfo.Top : 10;
        }

        private void SaveWindowStateInfo(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var set = new Models.WindowStateInfo(this);
            Settings.Default.MainWindowStateInfo = set;
            Settings.Default.Save();
        }
    }
}

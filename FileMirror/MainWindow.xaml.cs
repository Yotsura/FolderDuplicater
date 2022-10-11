using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileMirror
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _mvm = new MainWindowViewModel();
            this.DataContext = _mvm;
        }
        MainWindowViewModel _mvm;

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            _mvm.SaveSettings();
            MessageBox.Show("設定を保存しました。");
        }

        private void AddItem(object sender, RoutedEventArgs e)
        {
            _mvm.AddItem(origPath.Text, destPath.Text);
        }

        private void DelItem(object sender, RoutedEventArgs e)
        {
            _mvm.DelItem();
        }

        private void UpdItem(object sender, RoutedEventArgs e)
        {
            _mvm.UpdItem(origPath.Text, destPath.Text);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var test = (ListBox)sender;
            var test2 = test.SelectedItem;
            if (test2 == null) return;
            var selected= _mvm.splitedPair(test2.ToString());
            origPath.Text = selected.orig;
            destPath.Text = selected.dest;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mvm.RunMirroring();
            MessageBox.Show("ミラーリング完了");
        }
    }
}

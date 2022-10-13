using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FileMirror
{

    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        internal MainWindowViewModel()
        {
            var test = AppSettings.Default.PathPairs;
            _items.Clear();
            foreach (var pair in test)
            {
                _items.Add($"{pair.Orig}　→　{pair.Dest}");
            }
        }

        public (string orig,string dest) splitedPair(string orig)
        {
            string[] delimiter = { "　→　" };
            var sp = orig.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            return (sp[0], sp[1]);
        }

        public void RunMirroring()
        {
            var test = SplitedItems();
            foreach (var target in test)
            {
                new FolderDuplicater.Duplicater(target, true);
            }
        }

        public void SaveSettings()
        {
            var test = SplitedItems();
            AppSettings.Default.PathPairs = test.ToList();
            AppSettings.Default.Save();
        }

        public void DelItem()
        {
            this.Items.RemoveAt(this.CurrentIndex);
        }

        public void AddItem(string orig,string dest)
        {
            this.Items.Add($"{orig}　→　{dest}");
        }

        public void UpdItem(string orig,string dest)
        {
            //var val = $"{orig}　→　{dest}";
            this.Items.RemoveAt(this.CurrentIndex);
            this.Items.Add($"{orig}　→　{dest}");
        }

        private List<(string,string)> SplitedItems()
        {
            string[] delimiter = { "　→　" };
            var test = _items.Select(x =>
            {
                var sp = x.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                return (sp[0], sp[1]);
            });
            return test.ToList();
        }

        private ObservableCollection<string> _items = new ObservableCollection<string>();
        public ObservableCollection<string> Items { get { return this._items; } }

        private int _currentIndex;
        public int CurrentIndex
        {
            get { return this._currentIndex; }
            set { SetProperty(ref this._currentIndex, value); }
        }

        public Action<int> DropCallback { get { return OnDrop; } }

        private void OnDrop(int index)
        {
            if (index >= 0)
            {
                this.Items.Move(this.CurrentIndex, index);
            }
        }

        #region INotifyPropertyChanged のメンバ

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var h = this.PropertyChanged;
            if (h != null) h(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetProperty<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(target, value)) return false;
            target = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        #endregion INotifyPropertyChanged のメンバ


    }
}

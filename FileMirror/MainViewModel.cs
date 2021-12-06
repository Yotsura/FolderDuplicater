using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMirror
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _items = new ObservableCollection<string>()
        {
            @"C:\Users\pc-yuminaka\Desktop\excelVBAtest_20210929　→　C:\Users\pc-yuminaka\Desktop\送信先１",
            @"C:\Users\pc-yuminaka\Desktop\excelVBAtest_20210929　→　C:\Users\pc-yuminaka\Desktop\送信先２",
            "アイテム 3",
            "アイテム 4",
            "アイテム 5",
        };
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FileMirroringTool.Models;
using FileMirroringTool.Commands;
using System.Collections.ObjectModel;

namespace FileMirroringTool
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand MirrorListCtrlCommand { get; private set; }
        public MainWindowViewModel()
        {
            MirrorListCtrlCommand = new MirrorListCtrl(this);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private string _origPath = string.Empty;
        private string _destPath = string.Empty;
        //private MirrorInfo _selectedMirrorItem;
        private ObservableCollection<MirrorInfo> _mirrorList = new ObservableCollection<MirrorInfo>()
        {
            new MirrorInfo{DestPath="destpath1",OrigPath="origpath1",IsChecked=true},
            new MirrorInfo{DestPath="destpath2",OrigPath="origpath2"},
            new MirrorInfo{DestPath="destpath3",OrigPath="origpath3",IsChecked=true},
        };

        public string OrigPath
        {
            get => _origPath;
            set
            {
                _origPath = value;
                OnPropertyChanged(nameof(OrigPath));
            }
        }
        public string DestPath
        {
            get => _destPath;
            set
            {
                _destPath = value;
                OnPropertyChanged(nameof(DestPath));
            }
        }

        public ObservableCollection<MirrorInfo> MirrorList
        {
            get => _mirrorList;
            set
            {
                _mirrorList = value;
                OnPropertyChanged(nameof(MirrorList));
            }
        }
        #endregion
    }
}

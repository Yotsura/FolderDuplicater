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
        public ICommand SettingCtrlCommand { get; private set; }
        public MainWindowViewModel()
        {
            MirrorListCtrlCommand = new MirrorListCtrl(this);
            SettingCtrlCommand = new SettingCtrl(this);
            if (Settings.Default.MirrorList != null)
                MirrorList = new ObservableCollection<MirrorInfo>(Settings.Default.MirrorList);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _origPath = string.Empty;
        private string _destPath = string.Empty;
        private MirrorInfo _selectedMirrorInfo;
        private ObservableCollection<MirrorInfo> _mirrorList = new ObservableCollection<MirrorInfo>();

        public MirrorInfo SelectedMirrorInfo
        {
            get => _selectedMirrorInfo;
            set
            {
                _selectedMirrorInfo = value;
                OnPropertyChanged(nameof(SelectedMirrorInfo));
                OrigPath = value?.OrigPath ?? string.Empty;
                OnPropertyChanged(nameof(OrigPath));
                DestPath = value?.DestPath ?? string.Empty;
                OnPropertyChanged(nameof(DestPath));
            }
        }

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

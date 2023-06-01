using System;
using System.ComponentModel;
using System.Windows.Input;
using FileMirroringTool.Models;
using System.Collections.ObjectModel;
using FileMirroringTool.ViewModels.Commands;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FileMirroringTool.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand MirrorListCtrlCommand { get; private set; }
        public ICommand SettingCtrlCommand { get; private set; }
        public ICommand MirroringCommand { get; private set; }
        public ICommand OpenDialogCommand { get; private set; }
        public ICommand OpenExplorerCommand { get; private set; }
        public MainWindowViewModel()
        {
            MirrorListCtrlCommand = new MirrorListCtrl(this);
            SettingCtrlCommand = new SettingCtrl(this);
            MirroringCommand = new MirroringCtrl(this);
            OpenDialogCommand = new OpenDialogCtrl(this);
            OpenExplorerCommand = new OpenExplorer(this);
            if (Settings.Default.MirrorList != null)
                MirrorList = new ObservableCollection<MirrorInfo>(Settings.Default.MirrorList);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        #region MainWindowProps
        private MirrorInfo _selectedMirrorInfo;
        private ObservableCollection<MirrorInfo> _mirrorList = new ObservableCollection<MirrorInfo>();
        private string _autoIntervalStr = Settings.Default.AutomationIntervalStr;
        private bool _autoRunnning = false;

        private string _searchFile = string.Empty;
        private FileInfo _selectedBackupFile;
        private ObservableCollection<FileInfo> _backUpFileList = new ObservableCollection<FileInfo>();
        public string SearchFile
        {
            get => _searchFile;
            set
            {
                _searchFile = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(_searchFile) && new FileInfo(_searchFile).Exists)
                {
                    var files = new BackupFile(SelectedMirrorInfo.OrigPath, _searchFile);
                    var list = new ObservableCollection<FileInfo>(files.ExistsBackUpFiles);
                    BackUpFileList = list;
                }
                else
                    BackUpFileList = new ObservableCollection<FileInfo>();
                OnPropertyChanged(nameof(BackUpFileList));
            }
        }
        public ObservableCollection<FileInfo> BackUpFileList
        {
            get => _backUpFileList;
            set
            {
                _backUpFileList = value;
                OnPropertyChanged();
            }
        }
        public FileInfo SelectedBackupFile
        {
            get => _selectedBackupFile;
            set
            {
                _selectedBackupFile = value;
                OnPropertyChanged();
            }
        }

        public MirrorInfo SelectedMirrorInfo
        {
            get
            {
                if (_selectedMirrorInfo == null)
                    _selectedMirrorInfo = new MirrorInfo();
                return _selectedMirrorInfo;
            }
            set
            {
                _selectedMirrorInfo = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MirrorInfo> MirrorList
        {
            get => _mirrorList;
            set
            {
                _mirrorList = value;
                OnPropertyChanged();
            }
        }
        public bool AutoRunning
        {
            get => _autoRunnning;
            set
            {
                _autoRunnning = value;
                OnPropertyChanged();
            }
        }

        public string AutoIntervalStr
        {
            get => _autoIntervalStr;
            set
            {
                _autoIntervalStr = value;
                OnPropertyChanged();
            }
        }
        public double AutoInterval
        {
            get => double.TryParse(_autoIntervalStr, out var num) ? num : 1;
            set
            {
                AutoIntervalStr = value.ToString();
                OnPropertyChanged();
            }
        }
        #endregion

        #region ProgressDialogProps
        private double _fileCnt_Target = 0;
        private double _fileCnt_Checked = 0;
        public double FileCnt_Target
        {
            get => _fileCnt_Target;
            set
            {
                _fileCnt_Target = value;
                PrgVal = (int)Math.Floor(100 * (_fileCnt_Checked / _fileCnt_Target));
                PrgStr = $"{_fileCnt_Checked}/{_fileCnt_Target}";
                OnPropertyChanged(nameof(IsPrepareing));
            }
        }

        public double FileCnt_Checked
        {
            get => _fileCnt_Checked;
            set
            {
                _fileCnt_Checked = value;
                PrgVal = (int)Math.Floor(100 * (_fileCnt_Checked / _fileCnt_Target));
                PrgStr = $"{_fileCnt_Checked}/{_fileCnt_Target}";
            }
        }

        private int _prgVal = 0;
        public int PrgVal
        {
            get => _prgVal;
            set
            {
                _prgVal = value;
                OnPropertyChanged();
            }
        }
        private string _prgStr = "0/0";
        public string PrgStr
        {
            get => _prgStr;
            set
            {
                _prgStr = value;
                OnPropertyChanged();
            }
        }

        private string _prgTitle = "＜データチェック中＞";
        public string PrgTitle
        {
            get => _prgTitle;
            set
            {
                _prgTitle = value;
                OnPropertyChanged();
            }
        }

        private string _prgFileName = string.Empty;
        public string PrgFileName
        {
            get => _prgFileName;
            set
            {
                _prgFileName = value;
                OnPropertyChanged();
            }
        }
        
        public bool IsPrepareing => FileCnt_Target < 1;

        public void ResetPrgStat()
        {
            FileCnt_Target = 0;
            FileCnt_Checked = 0;
            PrgTitle = "＜データチェック中＞";
            PrgFileName = string.Empty;
        }
        #endregion
    }
}

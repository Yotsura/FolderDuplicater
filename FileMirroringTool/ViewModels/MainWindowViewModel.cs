﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using FileMirroringTool.Models;
using System.Collections.ObjectModel;
using FileMirroringTool.ViewModels.Commands;

namespace FileMirroringTool.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand MirrorListCtrlCommand { get; private set; }
        public ICommand SettingCtrlCommand { get; private set; }
        public ICommand MirroringCommand { get; private set; }
        public ICommand OpenDialogCommand { get; private set; }
        public MainWindowViewModel()
        {
            MirrorListCtrlCommand = new MirrorListCtrl(this);
            SettingCtrlCommand = new SettingCtrl(this);
            MirroringCommand = new MirroringCtrl(this);
            OpenDialogCommand = new OpenDialogCtrl(this);
            if (Settings.Default.MirrorList != null)
                MirrorList = new ObservableCollection<MirrorInfo>(Settings.Default.MirrorList);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region MainWindowProps
        private string _sort = string.Empty;
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

                Sort = value?.Sort.ToString() ?? string.Empty;
                OnPropertyChanged(nameof(Sort));
                OrigPath = value?.OrigPath ?? string.Empty;
                OnPropertyChanged(nameof(OrigPath));
                DestPath = value?.DestPathsStr ?? string.Empty;
                OnPropertyChanged(nameof(DestPath));
            }
        }

        public string Sort
        {
            get => _sort;
            set
            {
                _sort = value;
                OnPropertyChanged(nameof(Sort));
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

        #region ProgressDialogProps
        public int DelCnt { get; set; } = 0;
        public int AddCnt { get; set; } = 0;
        public int UpdCnt { get; set; } = 0;

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
                OnPropertyChanged(nameof(PrgVal));
            }
        }
        private string _prgStr = "0/0";
        public string PrgStr
        {
            get => _prgStr;
            set
            {
                _prgStr = value;
                OnPropertyChanged(nameof(PrgStr));
            }
        }

        private string _prgTitle = "＜作業内容＞ファイル名";
        public string PrgTitle
        {
            get => _prgTitle;
            set
            {
                _prgTitle = value;
                OnPropertyChanged(nameof(PrgTitle));
            }
        }

        private string _prgFileName = string.Empty;
        public string PrgFileName
        {
            get => _prgFileName;
            set
            {
                _prgFileName = value;
                OnPropertyChanged(nameof(PrgFileName));
            }
        }

        public void ResetPrgStat()
        {
            FileCnt_Target = 0;
            FileCnt_Checked = 0;
            PrgTitle = "＜リスト作成中＞";
            PrgFileName = string.Empty;
        }
        #endregion
    }
}
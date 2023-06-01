using FileMirroringTool.Utils;
using FileMirroringTool.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FileMirroringTool.Models
{
    public class MirrorInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        int _id = -1;
        int _sort = 0;
        bool _skipExclamation = false;
        bool _needBackup = false;
        bool _isChecked = false;
        string _origPath = string.Empty;
        string _destPathsStr = string.Empty;
        public int ID
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        public int Sort
        {
            get => _sort;
            set
            {
                _sort = value;
                OnPropertyChanged();
            }
        }
        public int SortPara => Sort > 0 ? (-Sort) : ID; //sortがある場合は無いものより前に設定
        public bool SkipExclamation
        {
            get => _skipExclamation;
            set
            {
                _skipExclamation = value;
                OnPropertyChanged();
            }
        }
        public bool NeedBackup
        {
            get => _needBackup;
            set
            {
                _needBackup = value;
                OnPropertyChanged();
            }
        }
        public BackupInfo BackupInfo => new BackupInfo(OrigPath, SkipExclamation);

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }
        public string OrigPath
        {
            get => _origPath;
            set
            {
                _origPath = value;
                OnPropertyChanged();
            }
        }
        public string DestPathsStr
        {
            get => _destPathsStr;
            set
            {
                _destPathsStr = value;
                OnPropertyChanged();
            }
        }

        public FileCount FileCounter { get; set; } = new FileCount();

        public List<string> ExistDestPathsList
            => DestPathsStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Where(x => !string.IsNullOrEmpty(x) && Directory.Exists(x)).ToList();

        public bool CanExecuteMirroring
            => Directory.Exists(OrigPath) && ExistDestPathsList.Count() > 0;

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            MirrorInfo record = (MirrorInfo)obj;
            var result = ID == record.ID &&
                Sort == record.Sort &&
                SkipExclamation == record.SkipExclamation &&
                NeedBackup == record.NeedBackup &&
                OrigPath == record.OrigPath &&
                DestPathsStr == record.DestPathsStr;
            return result;
        }

        public override int GetHashCode()
        {
            var hashCode = ID ^ Sort
                ^ SkipExclamation.GetHashCode()
                ^ NeedBackup.GetHashCode()
                ^ OrigPath.GetHashCode()
                ^ DestPathsStr.GetHashCode();

            return hashCode;
        }

        public void MirroringInvoke(MainWindowViewModel mwvm, CancellationToken token)
        {
            if (NeedBackup)
            {
                mwvm.PrgTitle = $"＜バックアップ中＞{OrigPath}";
                BackupInfo.RunAllBackup();
            }

            FileCounter = new FileCount();
            try
            {
                token.ThrowIfCancellationRequested();
                mwvm.PrgTitle = $"＜削除対象リストアップ中＞";
                var delList = ExistDestPathsList
                    .Select(destPath =>
                    (
                        dir: destPath,
                        files:
                            (SkipExclamation ?
                                FileUtils.GetAllFiles(destPath) :
                                Directory.EnumerateFiles(destPath, "*", System.IO.SearchOption.AllDirectories))
                            .Select(file => new FileData(OrigPath, destPath, file, false))
                            .Where(file => file.IsDeletedFile)
                            .ToArray()
                    )).ToArray();

                mwvm.PrgTitle = $"＜更新対象リストアップ中＞{OrigPath}";
                var updList =
                    (SkipExclamation ?
                        FileUtils.GetAllFiles(OrigPath) :
                        Directory.EnumerateFiles(OrigPath, "*", System.IO.SearchOption.AllDirectories))
                    .Where(path =>
                    {
                        try
                        {
                            var attr = File.GetAttributes(path);
                            if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden) return false;
                            if ((attr & FileAttributes.System) == FileAttributes.System) return false;
                            return true;
                        }
                        catch(Exception e)
                        {
                            System.Diagnostics.Debug.Print($"Exception: {e.GetType().Name}\r\n＞{path}");
                            return true; //ファイルが壊れている可能性？
                        }
                    }).ToArray();

                mwvm.FileCnt_Target = delList.SelectMany(x => x.files).Count() + updList.Count() * ExistDestPathsList.Count();

                foreach (var (dir, files) in delList)
                {
                    mwvm.PrgTitle = $"＜削除中＞{OrigPath} -> {dir}";
                    foreach (var file in files)
                    {
                        token.ThrowIfCancellationRequested();
                        mwvm.FileCnt_Checked++;
                        mwvm.PrgFileName = file.DestInfo.FullName;
                        if (file.TryDeleteDestFile())
                            FileCounter.DelCnt++;
                    }
                    //空フォルダの削除
                    FileUtils.DeleteEmptyDirs(dir);
                }

                foreach (var destPath_orig in ExistDestPathsList)
                {
                    var destPath = destPath_orig;
                    mwvm.PrgTitle = $"＜更新中＞{OrigPath} -> {destPath}";

                    foreach (var file in updList)
                    {
                        token.ThrowIfCancellationRequested();
                        mwvm.FileCnt_Checked++;
                        try
                        {
                            var data = new FileData(OrigPath, destPath_orig, file, true);
                            mwvm.PrgFileName = data.DestInfo.FullName;
                            if (data.IsUpdatedFile)
                            {
                                if (data.TryDupricateFile()) FileCounter.UpdCnt++;
                            }
                            else if (data.IsNewFile)
                            {
                                if (data.TryDupricateFile()) FileCounter.AddCnt++;
                            }
                            else continue;

                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Print($"Exception: {e.GetType().Name}\r\n＞{file}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                    System.Diagnostics.Debug.Print("■Mirroring was manualy stopeed.", e.Message);
                else
                    System.Diagnostics.Debug.Print("Exception: " + e.GetType().Name);
            }
        }
    }
}

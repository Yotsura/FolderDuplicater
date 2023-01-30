using FileMirroringTool.Utils;
using FileMirroringTool.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FileMirroringTool.Models
{
    public class MirrorInfo
    {
        public int ID { get; set; } = -1;
        public int Sort { get; set; } = 0;
        public int SortPara => Sort > 0 ? (-Sort) : ID; //sortがある場合は無いものより前に設定
        public bool SkipExclamation { get; set; } = false;
        public bool NeedBackup { get; set; } = false;
        public BackupInfo BackupInfo => new BackupInfo(OrigPath, SkipExclamation);

        public bool IsChecked { get; set; } = true;
        public string OrigPath { get; set; } = string.Empty;
        public string DestPathsStr { get; set; } = string.Empty;

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
                    ));



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
                        file.DeleteDestFile();
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
                            if (data.IsUpdatedFile) FileCounter.UpdCnt++;
                            else if (data.IsNewFile) FileCounter.AddCnt++;
                            else continue;
                            data.DupricateFile();
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

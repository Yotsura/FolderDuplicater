﻿using FileMirroringTool.Utils;
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
        public BackupManager BackupInfo => new BackupManager(OrigPath);

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
                BackupInfo.RunAllBackup(SkipExclamation);
            }

            FileCounter = new FileCount();
            try
            {
                token.ThrowIfCancellationRequested();
                //mwvm.PrgTitle = $"＜更新対象リストアップ中＞{OrigPath}";
                var fileList = new DirectoryInfo(OrigPath).GetAllFileInfos("*", SearchOption.AllDirectories, SkipExclamation).ToArray();

                foreach (var destPath_orig in ExistDestPathsList)
                {
                    mwvm.PrgTitle = $"＜更新中＞{OrigPath} -> {destPath_orig}";
                    var updList1 = fileList.Select(file => new FileData(OrigPath, destPath_orig, file.FullName, true)).Where(x => x.IsNewFile || x.IsUpdatedFile);
                    mwvm.FileCnt_Target = updList1.Count();
                    mwvm.FileCnt_Checked = 0;
                    updList1.AsParallel().ForAll(data =>
                    {
                        token.ThrowIfCancellationRequested();
                        try
                        {
                            mwvm.PrgFileName = data.DestInfo.FullName;
                            if (data.TryDupricateFile())
                                if (data.IsUpdatedFile)
                                    FileCounter.UpdCnt++;
                                else if (data.IsNewFile) FileCounter.AddCnt++;
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Print($"Exception: {e.GetType().Name}\r\n＞{data.OrigInfo.FullName}");
                        }
                        mwvm.FileCnt_Checked++;
                    });
                }

                //mwvm.PrgTitle = $"＜削除対象リストアップ中＞";
                mwvm.ResetPrgStat();
                var delList = ExistDestPathsList
                    .Select(destPath =>
                    (
                        dir: destPath,
                        files:
                            new DirectoryInfo(destPath).GetAllFileInfos("*", SearchOption.AllDirectories, SkipExclamation)
                            .Select(file => new FileData(OrigPath, destPath, file.FullName, false))
                            .Where(file => file.IsDeletedFile).ToArray()
                    ));
                mwvm.FileCnt_Target = delList.Sum(x => x.files.Count());
                foreach (var (dir, files) in delList)
                {
                    mwvm.PrgTitle = $"＜削除中＞{dir}";
                    files.AsParallel().ForAll(file =>
                    {
                        token.ThrowIfCancellationRequested();
                        mwvm.FileCnt_Checked++;
                        mwvm.PrgFileName = file.DestInfo.FullName;
                        if (file.TryDeleteDestFile())
                            FileCounter.DelCnt++;
                    });
                }
                mwvm.PrgTitle = $"＜空ディレクトリ削除中＞";

                ExistDestPathsList.ForEach(x => FileUtils.DeleteEmptyDirs(x));
                //ExistDestPathsList.SelectMany(dir => Directory.EnumerateDirectories(dir, "*", SearchOption.AllDirectories))
                //    .OrderBy(x => x.Length).ToList().ForEach(dir => FileUtils.DeleteEmptyDirs(dir));
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

﻿using FileMirroringTool.Extensions;
using FileMirroringTool.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileMirroringTool.Models
{
    public class BackupManager
    {
        public DirectoryInfo TargetDirectory { get; set; }
        public DirectoryInfo BackUpDirectory { get; set; }

        public BackupManager(string targetDirPath)
        {
            if (targetDirPath.IsNullOrEmpty()) return;
            TargetDirectory = new DirectoryInfo(targetDirPath);
            if (!TargetDirectory.Exists) return;
            BackUpDirectory = new DirectoryInfo(Path.Combine(TargetDirectory.Parent.FullName, $"!Backup_{TargetDirectory.Name}"));
        }

        public void RunAllBackup(bool skipExclamation = false)
        {
            var files = skipExclamation ?
                TargetDirectory.GetAllFileInfos_skipExclamation():
                TargetDirectory.EnumerateFiles("*", SearchOption.AllDirectories);
            files.ToList().ForEach(file => new BackupInfo(TargetDirectory, file).RunBackup(skipExclamation));
        }
    }

    public class BackupInfo
    {
        public FileInfo OrigFile { get; set; }

        public DirectoryInfo RootTargetDirInfo { get; set; }
        public DirectoryInfo RootBackupDirInfo { get; set; }
        
        DirectoryInfo BackUpDirInfo { get; set; }

        string GetBackupFilePath(string dateStr)
            => Path.Combine(BackUpDirInfo.FullName, $"{Path.GetFileNameWithoutExtension(OrigFile.Name)}_{dateStr}{OrigFile.Extension}");
        public BackupInfo(DirectoryInfo targetDir, FileInfo file)
        {
            Initialize(targetDir, file);
        }
        public BackupInfo(string targetDirPath, string filePath)
        {
            Initialize(new DirectoryInfo(targetDirPath), new FileInfo(filePath));
        }
        void Initialize(DirectoryInfo targetDir, FileInfo file)
        {
            OrigFile = file;
            RootTargetDirInfo = targetDir;
            RootBackupDirInfo = new DirectoryInfo(Path.Combine(RootTargetDirInfo.Parent.FullName, $"!Backup_{RootTargetDirInfo.Name}"));
            BackUpDirInfo = new DirectoryInfo(file.DirectoryName.GetRelativePath(RootTargetDirInfo.FullName, RootBackupDirInfo.FullName));

        }

        public List<FileInfo> GetBackupList(bool isAscending = true, bool skipExclamation = false)
        {
            if (!BackUpDirInfo.Exists)
                return new List<FileInfo>();
            var fileName = Path.GetFileNameWithoutExtension(OrigFile.Name);
            var pattern = $"{Path.GetFileNameWithoutExtension(OrigFile.Name)}_*";
            var backups =
                skipExclamation ?
                BackUpDirInfo.GetAllFileInfos_skipExclamation() :
                BackUpDirInfo.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly);
            return
                isAscending ?
                backups.OrderBy(x => x.CreationTime).ToList() :
                backups.OrderByDescending(x => x.CreationTime).ToList();
        }

        public void RunBackup(bool skipExclamation = false)
        {
            var runtime = DateTime.Now;
            var backups = GetBackupList(true, skipExclamation);
            if (backups.Count < 1)
            {
                //初回バックアップ
                var newpath = GetBackupFilePath("0000");
                OrigFile.SafeCopyTo(newpath);
            }
            else if (backups.Last().LastWriteTime < OrigFile.LastWriteTime)
            {
                var newpath = GetBackupFilePath(runtime.ToString("yyyyMMddHHmm"));
                OrigFile.SafeCopyTo(newpath);
            }
            var buffer = new Queue<FileInfo>(backups.Skip(1));
            while (buffer.Any())
            {
                var file = buffer.Dequeue();
                if (file.CreationTime < runtime.AddHours(-1))
                    //1時間以上経過しているファイル名を_yyyyMMddHHに変更する。
                    file.MoveTo(GetBackupFilePath(file.CreationTime.ToString("yyyyMMddHH")), true);
                else if (file.CreationTime < runtime.AddHours(-48))
                    //48時間以上経過しているファイル名を_yyyyMMddに変更する。
                    file.MoveTo(GetBackupFilePath(file.CreationTime.ToString("yyyyMMdd")), true);
                else if (file.CreationTime < runtime.AddMonths(-1))
                    //1ヶ月以上経過しているファイル名を_yyyyMMに変更する。
                    file.MoveTo(GetBackupFilePath(file.CreationTime.ToString("yyyyMM")), true);
                else if (file.CreationTime < runtime.AddMonths(-3))
                    //3ヶ月以上経過しているファイル名を_0000に変更する。
                    file.MoveTo(GetBackupFilePath(file.CreationTime.ToString("0000")), true);
            }
        }
    }
}
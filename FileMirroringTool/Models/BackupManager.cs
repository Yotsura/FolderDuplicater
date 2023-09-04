using FileMirroringTool.Extensions;
using FileMirroringTool.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
            var files = TargetDirectory.GetAllFileInfos("*", SearchOption.AllDirectories, skipExclamation)
                .Select(file => new BackupInfo(TargetDirectory, file)).ToList();
            files.ForEach(backupinfo => backupinfo.RunBackup(skipExclamation));

            //削除済みのファイルのバックアップも同様のリネーム処理したい。
            //ターゲットフォルダのうち、fileのバックアップではないものを除外
            var executedBackups = files.SelectMany(file => file.GetBackupList(true, skipExclamation)).ToList();
            var allbackups = new BackupInfo(TargetDirectory).RootBackupDirInfo
                .GetAllFileInfos("*", SearchOption.AllDirectories, skipExclamation);
            var temp = allbackups.Where(x => executedBackups.All(y => x.FullName != y.FullName));

            //ファイル名でグループ化して削除済みファイルパスを推測
            foreach (var file in temp.GroupBy(x => OmitBackUpDate(x.FullName)))
            {
                var filename = file.Key.GetRelativePath(BackUpDirectory.FullName, TargetDirectory.FullName);
                new BackupInfo(TargetDirectory, new FileInfo(filename)).RunBackup(skipExclamation);
            }
        }

        string OmitBackUpDate(string backfilename)
        {
            var name = Path.GetFileNameWithoutExtension(backfilename);
            if (!name.Contains("_")) return backfilename;
            var extension = Path.GetExtension(backfilename);
            var dir = Path.GetDirectoryName(backfilename);
            var sp = name.Split('_');
            var test = string.Join("_", sp.Take(sp.Length - 1));

            var origfilename = Path.Combine(dir, $"{test}{extension}");
            return origfilename;
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
        public BackupInfo(DirectoryInfo targetDir, FileInfo file =null)
        {
            Initialize(targetDir, file);
        }
        public BackupInfo(string targetDirPath, string filePath)
        {
            Initialize(new DirectoryInfo(targetDirPath), new FileInfo(filePath));
        }
        void Initialize(DirectoryInfo targetDir, FileInfo file = null)
        {
            RootTargetDirInfo = targetDir;
            RootBackupDirInfo = new DirectoryInfo(Path.Combine(RootTargetDirInfo.Parent.FullName, $"!Backup_{RootTargetDirInfo.Name}"));
            if (file == null) return;
            OrigFile = file;
            BackUpDirInfo = new DirectoryInfo(OrigFile.DirectoryName.GetRelativePath(RootTargetDirInfo.FullName, RootBackupDirInfo.FullName));
        }

        public List<FileInfo> GetBackupList(bool isAscending = true, bool skipExclamation = false)
        {
            if (!BackUpDirInfo.Exists)
                return new List<FileInfo>();
            var pattern = OrigFile == null ? "*" : $"{Path.GetFileNameWithoutExtension(OrigFile.Name)}_*";
            var backups = BackUpDirInfo.GetAllFileInfos(pattern, SearchOption.TopDirectoryOnly, skipExclamation);
            if (OrigFile != null)
            {
                //元々ファイル名にアンダーバーがあると余計なものも取得してきてしまうので除外する。
                var reg = new Regex(Path.GetFileNameWithoutExtension(OrigFile.Name) + @"_\d{4,12}\..+");
                backups = backups.Where(x => reg.IsMatch(x.Name));
            }
            return
                isAscending ?
                backups.OrderBy(x => x.CreationTime).ToList() :
                backups.OrderByDescending(x => x.CreationTime).ToList();
        }

        public void RunBackup(bool skipExclamation = false)
        {
            var runtime = DateTime.Now;
            var backups = GetBackupList(true, skipExclamation);
            //初回or最後のバックアップより更新されている場合は現状のファイルをバックアップに追加する。
            if (backups.Count < 1 || backups.Last().LastWriteTime < OrigFile.LastWriteTime)
                OrigFile.SafeCopyTo(GetBackupFilePath(runtime.ToString("yyyyMMddHHmm")));
            var buffer = new Queue<FileInfo>(backups);
            while (buffer.Any())
            {
                var file = buffer.Dequeue();
                if (file.CreationTime.AddMonths(3) < runtime)
                    //3ヶ月以上経過しているファイル名を_yyyyに変更する。
                    file.MoveTo(GetBackupFilePath(file.CreationTime.ToString("yyyy")), true);
                else if (file.CreationTime.AddMonths(1) < runtime)
                    //1ヶ月以上経過しているファイル名を_yyyyMMに変更する。
                    file.MoveTo(GetBackupFilePath(file.CreationTime.ToString("yyyyMM")), true);
                else if (file.CreationTime.AddHours(48) < runtime)
                    //48時間以上経過しているファイル名を_yyyyMMddに変更する。
                    file.MoveTo(GetBackupFilePath(file.CreationTime.ToString("yyyyMMdd")), true);
                else if (file.CreationTime.AddHours(1) < runtime)
                    //1時間以上経過しているファイル名を_yyyyMMddHHに変更する。
                    file.MoveTo(GetBackupFilePath(file.CreationTime.ToString("yyyyMMddHH")), true);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileMirroringTool.Models
{
    public class BackupInfo
    {
        public string TargetDirectory { get; set; } = string.Empty;
        public List<BackupFile> FileInfos { get; set; } = new List<BackupFile>();

        public BackupInfo(string targetDirectory)
        {
            TargetDirectory = targetDirectory;

            if (string.IsNullOrEmpty(targetDirectory)) return;
            
            var dir = new DirectoryInfo(targetDirectory);
            if (dir.Exists)
                FileInfos = Directory.EnumerateFiles(targetDirectory, "*", System.IO.SearchOption.AllDirectories)
                    .Select(x => new BackupFile(targetDirectory, x)).ToList();
        }

        public void RunAllBackup()
        {
            FileInfos.ForEach(x => x.RunBackup());
            //空ディレクトリ削除
            DeleteEmptyDirs(BackUpUtils.GetBackupRootDirName(new DirectoryInfo(TargetDirectory)));
        }

        static void DeleteEmptyDirs(string dir)
        {
            if (String.IsNullOrEmpty(dir))
                throw new ArgumentException(
                    "Starting directory is a null reference or an empty string",
                    "dir");

            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    DeleteEmptyDirs(d);
                }

                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        Directory.Delete(dir);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
        }
    }

    public class BackupFile
    {
        public DirectoryInfo TargetDir { get; set; }
        public FileInfo TargetFile { get; set; }
        public BackupDirectoryInfo MainBackupDir { get; set; }
        public List<BackupDirectoryInfo> HourBackupDirs { get; set; }
        public BackupFile(string targetDirPath, string targetFilePath)
        {
            TargetDir = new DirectoryInfo(targetDirPath);
            TargetFile = new FileInfo(targetFilePath);
            HourBackupDirs = Enumerable.Range(0, 4).Select(x => x * 0.25)
                .Concat(Enumerable.Range(1, 48).Select(x => (double)x))
                .Concat(Enumerable.Range(3, 5).Select(x => (double)x * 24))
                .Select(x => new BackupDirectoryInfo(TargetDir, x))
                .OrderByDescending(x => x.BackupHour).ToList();
            MainBackupDir = new BackupDirectoryInfo(TargetDir);
        }

        public List<BackupDirectoryInfo> AllBackupDirs
            => HourBackupDirs.Append(MainBackupDir).OrderByDescending(x => x.BackupHour).ToList();
        public List<FileInfo> ExistsBackUpFiles
            => AllBackupDirs.Select(dir => GetBacnUpFIleInfo(dir)).Where(x => x != null && x.Exists)
                .OrderByDescending(x => x.LastWriteTime).ToList();
        public DateTime LastBackupTime
            => ExistsBackUpFiles.FirstOrDefault()?.LastWriteTime ?? new DateTime(1000, 1, 1);

        public void RunBackup()
        {
            var runTime = DateTime.Now;

            //既存バックアップのフォルダ移動
            foreach (var sourceBackupFile in ExistsBackUpFiles)
            {
                //既存のバックアップファイルあり。更新日時が現在からどのくらい前か？
                var destFile = GetBacnUpFIleInfo(GetBackupDirectory(runTime - sourceBackupFile.LastWriteTime));//移動先のファイル情報
                if (sourceBackupFile.DirectoryName == destFile.Directory.FullName)
                    continue;//フォルダ移動しないならスキップ
                if (Directory.Exists(destFile.DirectoryName))
                {
                    if (destFile.Exists) destFile.Delete();//新規ファイルを優先。
                }
                else Directory.CreateDirectory(destFile.DirectoryName);
                sourceBackupFile.MoveTo(destFile.FullName);
            }

            if (LastBackupTime >= TargetFile.LastWriteTime) return;
            //新規バックアップの作成
            var orig_span = runTime - TargetFile.LastWriteTime;

            //最終的な更新日時が変わっていなければ更新は不要
            var newBackupFile = GetBacnUpFIleInfo(GetBackupDirectory(orig_span));
            if (newBackupFile == null) return;


            if (!Directory.Exists(newBackupFile.DirectoryName))
                Directory.CreateDirectory(newBackupFile.DirectoryName);

            TargetFile.CopyTo(newBackupFile.FullName, true);
        }

        BackupDirectoryInfo GetBackupDirectory(TimeSpan span)
            => AllBackupDirs.FirstOrDefault(x => x.BackupHour <= span.TotalHours);

        FileInfo GetBacnUpFIleInfo(BackupDirectoryInfo x) =>
            x == null ? null : new FileInfo(TargetFile.FullName.Replace(TargetDir.FullName, x.BackUpDir.FullName));
    }

    public class BackupDirectoryInfo
    {
        public double BackupHour { get; set; } = -1;
        public DirectoryInfo BackUpDir { get; set; }
        /// <summary>
        /// 指定時間前のバックアップフォルダを取得します。
        /// </summary>
        public BackupDirectoryInfo(DirectoryInfo sourcedir, double hour)
        {
            BackupHour = hour;
            BackUpDir = new DirectoryInfo(BackUpUtils.GetBackupDirName(sourcedir, BackupHour));
        }

        /// <summary>
        /// メインのバックアップディレクトリを取得します。
        /// </summary>
        public BackupDirectoryInfo(DirectoryInfo sourcedir)
        {
            BackupHour = 8 * 24;
            BackUpDir = new DirectoryInfo(BackUpUtils.GetBackupDirName(sourcedir));
        }
    }

    public static class BackUpUtils
    {
        /// <summary>
        /// 指定時間前のバックアップフォルダ名を取得します。
        /// </summary>
        public static string GetBackupDirName(this DirectoryInfo sourcedir, double hour)
            => Path.Combine(GetBackupRootDirName(sourcedir), hour.ToString() + "h");

        /// <summary>
        /// メインのバックアップディレクトリ名を取得します。
        /// </summary>
        public static string GetBackupDirName(this DirectoryInfo sourcedir)
            => Path.Combine(GetBackupRootDirName(sourcedir), "Main");

        /// <summary>
        /// バックアップ全体の親ディレクトリを取得します。
        /// </summary>
        public static string GetBackupRootDirName(this DirectoryInfo sourcedir)
            => Path.Combine(sourcedir.Parent.FullName, $"!Backup_{sourcedir.Name}");
    }
}

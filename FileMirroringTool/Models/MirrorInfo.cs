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
        //public string BackupSpans { get; set; } = string.Empty; //ファイルの更新周期、指定日数以内のファイルはスキップ

        //double[] SpanList => BackupSpans.Split(',').Select(x => double.TryParse(x, out var num) ? num : -1)
        //    .Where(x => x >= 0).Distinct().ToArray();
        public bool NeedBackup { get; set; } = false;
        public string BackupMode => NeedBackup ? "ON" : "OFF";

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
                BackupMode == record.BackupMode &&
                //BackupSpans == record.BackupSpans &&
                OrigPath == record.OrigPath &&
                DestPathsStr == record.DestPathsStr;
            return result;
        }

        public override int GetHashCode()
        {
            var hashCode = ID ^ Sort
                ^ BackupMode.GetHashCode()
                //^ BackupSpans.GetHashCode()
                ^ OrigPath.GetHashCode()
                ^ DestPathsStr.GetHashCode();

            return hashCode;
        }

        public void MirroringInvoke(MainWindowViewModel mwvm, CancellationToken token)
        {
            FileCounter = new FileCount();
            token.ThrowIfCancellationRequested();
            try
            {
                var delList = ExistDestPathsList
                    .Select(destPath =>
                    (
                        dir: destPath,
                        files: Directory.EnumerateFiles(destPath, "*", SearchOption.AllDirectories)
                            .Select(file => new FileData(OrigPath, destPath, file, false))
                            .Where(file => file.IsDeletedFile)
                            .ToArray()
                    ));
                var updList =
                    Directory.EnumerateFiles(OrigPath, "*", SearchOption.AllDirectories)
                    .Where(file =>
                    {
                        var attr = File.GetAttributes(file);
                        if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden) return false;
                        if ((attr & FileAttributes.System) == FileAttributes.System) return false;
                        return true;
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
                }

                foreach (var destPath_orig in ExistDestPathsList)
                {
                    var destPath = destPath_orig;
                    mwvm.PrgTitle = $"＜更新中＞{OrigPath} -> {destPath}";

                    foreach (var file in updList)
                    {
                        token.ThrowIfCancellationRequested();
                        mwvm.FileCnt_Checked++;

                        var data = new FileData(OrigPath, destPath_orig, file, true);
                        mwvm.PrgFileName = data.DestInfo.FullName;
                        if (data.IsUpdatedFile) FileCounter.UpdCnt++;
                        else if (data.IsNewFile) FileCounter.AddCnt++;
                        else continue;
                        data.DupricateFile();
                    }
                }
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                    System.Diagnostics.Debug.Print("■Mirroring was manualy stopeed.", e.Message);
                else
                    Console.WriteLine("Exception: " + e.GetType().Name);
            }
        }
    }
}

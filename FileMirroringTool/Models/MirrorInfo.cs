using FileMirroringTool.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileMirroringTool.Models
{
    public class MirrorInfo
    {
        public int ID { get; set; } = -1;
        public int Sort { get; set; } = 0;
        public int SortPara => Sort > 0 ? (-Sort) : ID; //sortがある場合は無いものより前に設定
        public bool IsChecked { get; set; } = true;
        public string OrigPath { get; set; } = string.Empty;
        public string DestPathsStr { get; set; } = string.Empty;

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
                OrigPath == record.OrigPath &&
                DestPathsStr == record.DestPathsStr;
            return result;
        }

        public override int GetHashCode()
        {
            var hashCode = ID ^ Sort
                ^ OrigPath.GetHashCode()
                ^ DestPathsStr.GetHashCode();

            return hashCode;
        }

        public void MirroringInvoke(MainWindowViewModel mwvm)
        {
            mwvm.DelCnt = 0;
            mwvm.AddCnt = 0;
            mwvm.UpdCnt = 0;
            mwvm.FileCnt_Checked = 0;
            try
            {
                var delList = ExistDestPathsList
                    .Select(destPath =>
                    (
                        dir: destPath,
                        files: Directory.EnumerateFiles(destPath, "*", SearchOption.AllDirectories)
                            .OrderByDescending(x => x).ToArray()
                    ));
                var updList =
                    Directory.EnumerateFiles(OrigPath, "*", System.IO.SearchOption.AllDirectories)
                    .OrderByDescending(x => x).ToArray();
                mwvm.FileCnt_Target = delList.SelectMany(x => x.files).Count() + updList.Count() * ExistDestPathsList.Count();

                foreach (var (dir, files) in delList)
                {
                    mwvm.PrgTitle = $"＜削除中＞{OrigPath} -> {dir}";
                    foreach (var file in files)
                    {
                        mwvm.FileCnt_Checked++;
                        var data = new FileData(OrigPath, dir, file, false);
                        mwvm.PrgFileName = data.DestInfo.FullName;
                        if (!data.IsDeletedFile) continue;
                        data.DeleteDestFile();
                        mwvm.DelCnt++;
                    }
                }

                foreach (var destPath in ExistDestPathsList)
                {
                    mwvm.PrgTitle = $"＜更新中＞{OrigPath} -> {destPath}";
                    foreach (var file in updList)
                    {
                        mwvm.FileCnt_Checked++;
                        var data = new FileData(OrigPath, destPath, file, true);
                        mwvm.PrgFileName = data.DestInfo.FullName;
                        if (data.IsUpdatedFile) mwvm.UpdCnt++;
                        else if (data.IsNewFile) mwvm.AddCnt++;
                        else continue;
                        data.DupricateFile();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

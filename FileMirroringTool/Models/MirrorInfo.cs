using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int FileCnt_Target { get; set; } = 0;
        public int FileCnt_Checked { get; set; } = 0;

        public List<string> DestPathsList
        {
            get => DestPathsStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Where(x => x != string.Empty).ToList();
            set => DestPathsStr = string.Join("\r\n", value);
        }

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

        public void MirroringInvoke()
        {
            var existDestPath = DestPathsList.Where(x => Directory.Exists(x)).ToList();
            if (!Directory.Exists(OrigPath) || existDestPath.Count() < 1)
                return;
            try
            {
                var delList = existDestPath
                    .Select(destPath =>
                    (
                        dir: destPath,
                        files: Directory.EnumerateFiles(destPath, "*", SearchOption.AllDirectories)
                            .OrderByDescending(x => x).ToArray()
                    ));
                var updList =
                    Directory.EnumerateFiles(OrigPath, "*", System.IO.SearchOption.AllDirectories)
                    .OrderByDescending(x => x).ToArray();
                FileCnt_Target = delList.SelectMany(x => x.files).Count() + updList.Count();

                foreach (var (dir, files) in delList)
                {
                    foreach (var file in files)
                    {
                        FileCnt_Checked++;
                        var data = new FileData(OrigPath, dir, file, false);
                        if (data.IsDeletedFile)
                            data.DeleteDestFile();
                    }
                }

                foreach (var destPath in DestPathsList)
                {
                    foreach (var file in updList)
                    {
                        FileCnt_Checked++;
                        var data = new FileData(OrigPath, destPath, file, true);
                        if (data.IsNewFile || data.IsUpdatedFile)
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

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
                foreach (var destPath in existDestPath)
                {
                    DeleteNotExistFiles(destPath);
                }
                UpdFiles();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void DeleteNotExistFiles(string destPath)
        {
            var delList =
                Directory.EnumerateFiles(destPath, "*", System.IO.SearchOption.AllDirectories)
                .Select(file => new FileData(OrigPath, destPath, file, false))
                .Where(x => x.IsDeletedFile).OrderByDescending(x => x.DestInfo.FullName.Length).ToList();
            delList.AsParallel().ForAll(file => file.DeleteDestFile());
        }

        void UpdFiles()
        {
            var allfiles =
                Directory.EnumerateFiles(OrigPath, "*", System.IO.SearchOption.AllDirectories)
                .Select(file => DestPathsList.Select(d => new FileData(OrigPath, d, file, true))
                    .Where(x => x.IsNewFile || x.IsUpdatedFile))
                .SelectMany(x => x).ToList();
            allfiles.AsParallel().ForAll(file => file.DupricateFile());
        }
    }
}

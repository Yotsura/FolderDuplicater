using System.IO;
using System.Linq;

namespace FileMirroringTool.Models
{
    internal class FileData
    {
        public bool IsNewFile { get; set; }
        public bool IsUpdatedFile { get; set; }
        public bool IsDeletedFile { get; set; }
        public FileInfo OrigInfo { get; set; }
        public FileInfo DestInfo { get; set; }

        public FileData(string origPath, string destPath, string filepath, bool isOrig)
        {
            if (isOrig)
            {
                OrigInfo = new FileInfo(filepath);
                DestInfo = new FileInfo(OrigInfo.FullName.Replace(origPath, destPath));
            }
            else
            {
                DestInfo = new FileInfo(filepath);
                OrigInfo = new FileInfo(DestInfo.FullName.Replace(destPath, origPath));
            }

            //コピー元にのみ存在するか？
            IsNewFile = OrigInfo.Exists && !DestInfo.Exists;
            //コピー先にのみ存在するか？
            IsDeletedFile = !OrigInfo.Exists && DestInfo.Exists;
            //どちらにもあるなら更新されているか？
            if (DestInfo.Exists && OrigInfo.Exists)
                IsUpdatedFile =
                    DestInfo.LastWriteTimeUtc < OrigInfo.LastWriteTimeUtc ||
                    DestInfo.Length != OrigInfo.Length;
        }

        public void DupricateFile()
        {
            if (!Directory.Exists(DestInfo.DirectoryName))
                Directory.CreateDirectory(DestInfo.DirectoryName);
            File.Copy(OrigInfo.FullName, DestInfo.FullName, true);
        }

        public void DeleteDestFile()
        {
            if (File.Exists(DestInfo.FullName)) File.Delete(DestInfo.FullName);
            //空のフォルダをさかのぼって削除
            DelDir(DestInfo.DirectoryName);
        }

        void DelDir(string dir)
        {
            if (Directory.EnumerateFileSystemEntries(dir, "*", System.IO.SearchOption.AllDirectories).Any()) return;
            Directory.Delete(dir, false);
            DelDir(new DirectoryInfo(dir).Parent.FullName);
        }
    }
}

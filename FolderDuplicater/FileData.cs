using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FolderDuplicater
{
    class FileData
    {
        public bool IsNewFile { get; set; }
        public bool IsUpdatedFile { get; set; }
        public bool IsDeletedFile { get; set; }
        public FileInfo OrigInfo { get; set; }
        public FileInfo DestInfo { get; set; }

        public FileData((string origPath, string destPath) target, string filepath, bool isOrig)
        {
            if (isOrig)
            {
                OrigInfo = new FileInfo(filepath);
                DestInfo = new FileInfo(OrigInfo.FullName.Replace(target.origPath, target.destPath));
            }
            else
            {
                DestInfo = new FileInfo(filepath);
                OrigInfo = new FileInfo(DestInfo.FullName.Replace(target.destPath, target.origPath));
            }

            //コピー元にのみ存在するか？
            IsNewFile = !File.Exists(DestInfo.FullName);
            //どちらにもあるなら更新されているか？
            IsUpdatedFile = new FileInfo(DestInfo.FullName).LastWriteTimeUtc < OrigInfo.LastWriteTimeUtc;
            //コピー元にのみ存在するか？
            IsDeletedFile = File.Exists(DestInfo.FullName) && !File.Exists(OrigInfo.FullName);
        }

        public void DupricateFile()
        {
            if (!Directory.Exists(DestInfo.DirectoryName)) Directory.CreateDirectory(DestInfo.DirectoryName);
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

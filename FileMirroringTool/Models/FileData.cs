using System;
using System.IO;
using System.Linq;
using FileMirroringTool.Extensions;

namespace FileMirroringTool.Models
{
    internal class FileData
    {
        public bool IsNewFile => OrigInfo.Exists && !DestInfo.Exists;
        public bool IsUpdatedFile => DestInfo.Exists && OrigInfo.Exists
            && DestInfo.LastWriteTime < OrigInfo.LastWriteTime;
        public bool IsDeletedFile => !OrigInfo.Exists && DestInfo.Exists;
        /// <summary>
        /// ミラー元ファイル
        /// </summary>
        public FileInfo OrigInfo { get; set; }
        /// <summary>
        /// ミラー先ファイル
        /// </summary>
        public FileInfo DestInfo { get; set; }

        public FileData(string origPath, string destPath, string filepath, bool isOrig)
        {
            if (isOrig)
            {
                OrigInfo = new FileInfo(filepath);
                DestInfo = new FileInfo(filepath.GetRelativePath(origPath, destPath));
            }
            else
            {
                DestInfo = new FileInfo(filepath);
                OrigInfo = new FileInfo(filepath.GetRelativePath(destPath, origPath));
            }
        }

        public bool TryDupricateFile()
        {
            var result = false;
            if (!Directory.Exists(DestInfo.DirectoryName))
                Directory.CreateDirectory(DestInfo.DirectoryName);
            try
            {
                File.Copy(OrigInfo.FullName, DestInfo.FullName, true);
                result = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print($"Exception:{e.GetType().Name}\r\n{DestInfo.FullName}");
            }
            return result;
        }

        public bool TryDeleteDestFile()
        {
            var result = false;
            if (File.Exists(DestInfo.FullName))
            {
                try
                {
                    File.Delete(DestInfo.FullName);
                    result = true;
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.Print($"Exception:{e.GetType().Name}\r\n{DestInfo.FullName}");
                }
            }
            //空のフォルダをさかのぼって削除
            DelDir(DestInfo.DirectoryName);
            return result;
        }

        void DelDir(string dir)
        {
            if (!Directory.Exists(dir)) return;
            if (Directory.EnumerateFileSystemEntries(dir, "*", System.IO.SearchOption.AllDirectories).Any()) return;
            try
            {
                Directory.Delete(dir, false);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print($"Exception:{e.GetType().Name}\r\n{dir}");
            }
            DelDir(new DirectoryInfo(dir).Parent.FullName);
        }
    }
}

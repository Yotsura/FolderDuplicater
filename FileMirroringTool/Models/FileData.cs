using System;
using System.IO;
using System.Linq;
using FileMirroringTool.Extensions;
using FileMirroringTool.Utils;

namespace FileMirroringTool.Models
{
    internal class FileData
    {
        public readonly bool IsNewFile;
        public readonly bool IsUpdatedFile;
        public readonly bool IsDeletedFile;

        /// <summary>
        /// ミラー元ファイル
        /// </summary>
        public readonly FileInfo OrigInfo;
        /// <summary>
        /// ミラー先ファイル
        /// </summary>
        public readonly FileInfo DestInfo;

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
            IsNewFile = OrigInfo.Exists && !DestInfo.Exists;
            IsUpdatedFile = DestInfo.Exists && OrigInfo.Exists
                && DestInfo.LastWriteTime < OrigInfo.LastWriteTime;
            IsDeletedFile = !OrigInfo.Exists && DestInfo.Exists;
        }

        public bool TryDupricateFile(int encryptMode)
        {
            var result = false;
            if (!Directory.Exists(DestInfo.DirectoryName))
                Directory.CreateDirectory(DestInfo.DirectoryName);
            try
            {
                switch(encryptMode)
                {
                    case 1:
                        EncryptUtils.EncryptFile(OrigInfo.FullName, DestInfo.FullName, Settings.Default.EncryptKey);
                        break;
                    case 2:
                        EncryptUtils.DecryptFile(OrigInfo.FullName, DestInfo.FullName, Settings.Default.EncryptKey);
                        break;
                    default:
                        File.Copy(OrigInfo.FullName, DestInfo.FullName, true);
                        break;
                }
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

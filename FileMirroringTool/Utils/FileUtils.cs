using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileMirroringTool.Utils
{
    public static class FileUtils
    {
        public static void DeleteEmptyDirs(this string dir)
        {
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentException(
                    "Starting directory is a null reference or an empty string",
                    "dir");
            if (!Directory.Exists(dir)) return;
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

        /// <summary>
        /// 指定ディレクトリ以下で!で始まらないファイル/フォルダ以外すべて取得
        /// </summary>
        public static IEnumerable<FileInfo> GetAllFileInfos(this DirectoryInfo targetDirectory, string pattern = "*"
            ,SearchOption searchOption = SearchOption.TopDirectoryOnly , bool skipExclamation = false)
        {
            if (!targetDirectory.Exists) return Enumerable.Empty<FileInfo>();
            var files = targetDirectory.EnumerateFiles(pattern, searchOption).Where(file =>
            {
                try
                {
                    var attr = file.Attributes;
                    //隠しファイル・システムファイルを除外
                    if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden) return false;
                    if ((attr & FileAttributes.System) == FileAttributes.System) return false;
                    return true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Print($"Exception: {e.GetType().Name}\r\n＞{file.FullName}");
                    return true; //ファイルが壊れている可能性？
                }
            }).ToArray();
            return skipExclamation ?
                files.Where(file => !file.FullName.Substring(targetDirectory.FullName.Length).Contains(@"\!"))
                : files;
        }

        /// <summary>
        /// 先頭バイトから読み取れる拡張子と実際のファイル名の拡張子が食い違う場合、修正すべき正しいフルパスを返す。
        /// </summary>
         public static bool ShouldFixImgFileExtension(this string sourceFilePath, out string fixedPath)
         {
            fixedPath = string.Empty;
            if (string.IsNullOrEmpty(sourceFilePath)) return false;

            var origDirName = Path.GetDirectoryName(sourceFilePath);
            var origFileName = Path.GetFileNameWithoutExtension(sourceFilePath);

            var origExtension = Path.GetExtension(sourceFilePath).ToLower();
            var dataExtension = sourceFilePath.GetImgExtension().ToLower();
            if(string.IsNullOrEmpty(dataExtension)) return false;

            var ext_orig = ExtensionMapping.ContainsKey(origExtension) ? ExtensionMapping[origExtension] : string.Empty;
            var ext_data = ExtensionMapping.ContainsKey(dataExtension) ? ExtensionMapping[dataExtension] : string.Empty;

            fixedPath = Path.Combine(origDirName, origFileName + ext_data);
            return ext_orig != ext_data;
        }

        public static bool IsImgFile(this string path)
            => ExtensionMapping.ContainsKey(Path.GetExtension(path).ToLower());

        static readonly Dictionary<string, string> ExtensionMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { ".jpg", ".jpg" },
            { ".jpeg", ".jpg" },
            { ".png", ".png" },
            { ".gif", ".gif" },
            { ".bmp", ".bmp" },
            { ".tif", ".tif" },
            { ".tiff", ".tif" }
        };

        /// <summary>
        /// 画像ファイル先頭バイトから正しい拡張子を取得する。
        /// </summary>
        static string GetImgExtension(this string imgPath)
        {
            var imgData = File.ReadAllBytes(imgPath);
            // 先頭バイトの値に基づいて画像形式を判別
            if (imgData.Length > 1)
            {
                if (imgData[0] == 0xFF && imgData[1] == 0xD8)
                    return ".jpg";
                if (imgData[0] == 0x89 && imgData[1] == 0x50 && imgData[2] == 0x4E && imgData[3] == 0x47)
                    return ".png";
                if (imgData[0] == 0x47 && imgData[1] == 0x49 && imgData[2] == 0x46 && imgData[3] == 0x38)
                    return ".gif";
                if (imgData[0] == 0x42 && imgData[1] == 0x4D)
                    return ".bmp";
                if (imgData[0] == 0x49 && imgData[1] == 0x49 && imgData[2] == 0x2A && imgData[3] == 0x00)
                    return ".tif"; // TIFF形式 (Little Endian)
                if (imgData[0] == 0x4D && imgData[1] == 0x4D && imgData[2] == 0x00 && imgData[3] == 0x2A)
                    return ".tif"; // TIFF形式 (Big Endian)
            }

            return string.Empty;
        }

        /// <summary>
        /// 使用中のファイル含め、ロックしないようにsourceFileでdestFileを上書きします。ディレクトリが存在しなければ作成します。
        /// </summary>
        public static void SafeCopyTo(this FileInfo sourceFileInfo, string destFilePath)
            => sourceFileInfo.SafeCopyTo(new FileInfo(destFilePath));

        /// <summary>
        /// 使用中のファイル含め、ロックしないようにsourceFileでdestFileを上書きします。ディレクトリが存在しなければ作成します。
        /// </summary>
        public static void SafeCopyTo(this FileInfo sourceFileInfo, FileInfo destFileInfo)
        {
            if (!sourceFileInfo.Exists) return;
            try
            {
                // 操作中のファイルをロックせずにコピーするため、FileStreamを使う
                using (FileStream sourceStream = new FileStream(sourceFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // コピー先のフォルダが存在しない場合は作成する
                    if (!Directory.Exists(destFileInfo.DirectoryName))
                    {
                        Directory.CreateDirectory(destFileInfo.DirectoryName);
                    }

                    // ファイルをコピー
                    using (FileStream destinationStream = new FileStream(destFileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            destinationStream.Write(buffer, 0, bytesRead);
                        }
                    }
                    //最終更新日時を比較しているのでコピー先のファイルを修正する。※CopyToと同じ挙動にする。
                    if (destFileInfo.LastWriteTime != sourceFileInfo.LastWriteTime)
                        destFileInfo.LastWriteTime = sourceFileInfo.LastWriteTime;
                }

                Console.WriteLine("ファイルのコピーが成功しました。");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"ファイルのコピー中にエラーが発生しました: {ex.Message}");
            }
        }

        /// <summary>
        /// sourceFileをdestFileの位置へ移動します。destPathにファイルが存在する場合、overwiteがオンの場合に上書きします。sourceFileとdestFileが同じファイルを指定している場合、処理を行いません。
        /// </summary>
        public static void MoveTo(this FileInfo sourceFile ,string destFilePath , bool overwrite)
        {
            if (sourceFile.FullName == destFilePath)
                return;
            if (File.Exists(destFilePath))
            {
                if (overwrite)
                    File.Delete(destFilePath);
                else
                    return;
            }
            sourceFile.MoveTo(destFilePath);
        }
    }
}

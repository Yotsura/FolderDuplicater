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

        /// <summary>
        /// 指定ディレクトリ以下で!で始まらないファイル/フォルダ以外すべて取得
        /// </summary>
        public static string[] GetAllFiles(this string targetDirectory)
        {
            var dirs = Directory.EnumerateDirectories(targetDirectory, "*", SearchOption.TopDirectoryOnly)
                .Where(path => !path.Replace(targetDirectory, string.Empty).Contains(@"\!")).ToArray();
            var files = Directory.EnumerateFiles(targetDirectory, "*", SearchOption.TopDirectoryOnly).ToArray();
            if (files.Length < 1 && dirs.Length < 1)
                return files;
            var children = dirs.Select(dir => GetAllFiles(dir)).SelectMany(x => x).ToArray();
            var result = files.Concat(children).ToArray();
            return result;
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
    }
}

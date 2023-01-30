using System;
using System.IO;
using System.Linq;

namespace FileMirroringTool.Utils
{
    public static class FileUtils
    {
        public static void DeleteEmptyDirs(string dir)
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

        public static string[] GetAllFiles(string targetDirectory)
        {
            var dirs = Directory.EnumerateDirectories(targetDirectory, "*", SearchOption.TopDirectoryOnly)
                .Where(path => !path.Contains(@"\!")).ToArray();
            var files = Directory.EnumerateFiles(targetDirectory, "*", SearchOption.TopDirectoryOnly).ToArray();
            if (files.Length < 1 && dirs.Length < 1)
                return files;
            var children = dirs.Select(dir => GetAllFiles(dir)).SelectMany(x => x).ToArray();
            var result = files.Concat(children).ToArray();
            return result;
        }
    }
}

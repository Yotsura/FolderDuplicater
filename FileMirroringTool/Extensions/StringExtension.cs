using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMirroringTool.Extensions
{
    internal static class StringExtension
    {
        internal static bool IsNullOrEmpty(this string orig)
            => string.IsNullOrEmpty(orig);

        internal static string GetRelativePath(this string fullPath, string basePath, string destPath)
            => Path.Combine(destPath.TrimEnd(Path.DirectorySeparatorChar), fullPath.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar));
    }
}

using System.IO;

namespace FileMirroringTool.Utils
{

    public static class BackUpUtils
    {
        /// <summary>
        /// 指定時間前のバックアップフォルダ名を取得します。
        /// </summary>
        public static string GetBackupDirName(this DirectoryInfo sourcedir, double hour)
            => Path.Combine(GetBackupRootDirName(sourcedir), hour.ToString() + "h");

        /// <summary>
        /// メインのバックアップディレクトリ名を取得します。
        /// </summary>
        public static string GetBackupDirName(this DirectoryInfo sourcedir)
            => Path.Combine(GetBackupRootDirName(sourcedir), "Main");

        /// <summary>
        /// バックアップ全体の親ディレクトリを取得します。
        /// </summary>
        public static string GetBackupRootDirName(this DirectoryInfo sourcedir)
            => Path.Combine(sourcedir.Parent.FullName, $"!Backup_{sourcedir.Name}");
    }
}

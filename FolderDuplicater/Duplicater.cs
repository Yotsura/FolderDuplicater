using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FolderDuplicater
{
    class Duplicater
    {
        List<FileInfo> _origData { get; set; }
        string _origFolderPath { get; set; }
        string _destinationFolderPath { get; set; }

        public Duplicater(PathInfo pathinfo)
        {
            _origFolderPath = pathinfo.OrigPath;
            _destinationFolderPath = pathinfo.DestinationPath;
            if (!Directory.Exists(_origFolderPath))
            {
                Console.WriteLine("複製元フォルダーが存在しません。");
                return;
            }
            if (!Directory.Exists(_destinationFolderPath))
            {
                Console.WriteLine("複製先フォルダーが存在しません。");
                return;
            }
            Console.WriteLine($"\r\nファイルの読み込みを開始します。\r\nソート順：更新日降順→作成日時降順");
            _origData = Directory.EnumerateFiles(_origFolderPath, "*", System.IO.SearchOption.AllDirectories)
                .Select(file => new FileInfo(file)).OrderByDescending(x => x.LastWriteTimeUtc)
                .ThenByDescending(x => x.CreationTimeUtc).ToList();
                //.Select(file => new FileInfo(file)).OrderByDescending(x => x.CreationTimeUtc)
                //.ThenByDescending(x => x.LastWriteTimeUtc).ToList();
            Console.WriteLine($"\r\n{_origData.Count}件のファイルが存在します。");
        }

        public void Dupticate()
        {
            var top = Console.CursorTop;
            var left = Console.CursorLeft;
            Console.WriteLine($"\r\n<複製中>");
            var newcnt = 0;
            var updcnt = 0;
            _origData.AsParallel().ForAll(file =>
            {
                var targetFolderPath = CreateDesinationPath(file.Directory.ToString());
                if (!Directory.Exists(targetFolderPath))
                    Directory.CreateDirectory(targetFolderPath);
                var destinationPath = CreateDesinationPath(file.FullName);
                if (File.Exists(destinationPath))
                {
                    var destFile = new FileInfo(destinationPath);
                    if (destFile.LastWriteTimeUtc >= file.LastWriteTimeUtc) return;
                    updcnt++;
                }
                File.Copy(file.FullName, destinationPath, true);
                newcnt++;
            });
            Console.SetCursorPosition(left, top);
            Console.WriteLine($"\r\n<複製完了>\r\n{_origData.Count()}件の内、\r\n更新ファイル{updcnt}件、\r\n新規ファイル{newcnt - updcnt}件");
        }

        string CreateDesinationPath(string origfilepath)
        {
            return origfilepath.Replace(_origFolderPath, _destinationFolderPath);
        }
    }
}

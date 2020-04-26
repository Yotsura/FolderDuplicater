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
        List<FileData> _allfiles { get; set; }
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

            var top = Console.CursorTop;
            var left = Console.CursorLeft;
            Console.WriteLine($"\r\n更新対象のリストアップ中...");
            _allfiles = Directory.EnumerateFiles(_origFolderPath, "*", System.IO.SearchOption.AllDirectories)
                .AsParallel().Select(file => new FileData(pathinfo, file))
                .Where(x => x.IsNewFile || x.IsUpdatedFile).ToList();
            //System.Threading.Thread.Sleep(3000);

            Console.SetCursorPosition(left, top);

            var newfilecnt = _allfiles.Count(x => x.IsNewFile);
            var updatedfilecnt = _allfiles.Count(x => x.IsUpdatedFile);
            Console.WriteLine($"\r\n更新対象は全{_allfiles.Count}件です。　　　　");
            Console.WriteLine($"更新ファイル：{updatedfilecnt}件\r\n新規ファイル：{newfilecnt}件");
        }

        public void Dupticate()
        {
            var top = Console.CursorTop;
            var left = Console.CursorLeft;
            Console.WriteLine($"\r\n<複製中>");
            //System.Threading.Thread.Sleep(3000);
            _allfiles.AsParallel().ForAll(file =>
            {
                file.DupricateFile();
            });
            Console.SetCursorPosition(left, top);
            Console.WriteLine($"\r\n<複製完了>");
        }
    }
}

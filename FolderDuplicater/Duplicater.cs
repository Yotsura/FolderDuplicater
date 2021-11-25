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
        bool IsMirroring { get; set; }

        public Duplicater((string origPath,string destPath)target,bool isMirroring)
        {
            _origFolderPath = target.origPath;
            _destinationFolderPath = target.destPath;
            IsMirroring = isMirroring;

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
            Console.WriteLine("***********************************************************");
            Console.WriteLine($"複製元のフォルダーのパス：{_origFolderPath}\r\n複製先のフォルダーのパス：{_destinationFolderPath}\r\n");
            Console.WriteLine("***********************************************************");


            if (isMirroring) DeleteNotExistFiles();
            UpdFiles();
        }

        void DeleteNotExistFiles()
        {
            Console.WriteLine($"\r\n削除対象のリストアップ中...");
            var checkList = Directory.EnumerateFiles(_destinationFolderPath, "*", System.IO.SearchOption.AllDirectories);
            var checkListCnt = checkList.Count();

            var cnt = 0;
            int prePos = Console.CursorLeft;//現在カーソル位置を取得

            var delList = checkList
                .Select(file =>
                {
                    cnt++;
                    var temp = new FileData((_origFolderPath, _destinationFolderPath), file, false);
                    var per = (double)cnt / (double)checkListCnt;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(($"{GetProgressBar(20, per)}{cnt}/{checkListCnt}").PadRight(prePos));

                    return temp;
                }).Where(x => x.IsDeletedFile).ToList();
            delList.AsParallel().ForAll(file => file.DeleteDestFile());
            Console.WriteLine("<削除完了>");
        }

        void UpdFiles()
        {
            Console.WriteLine($"\r\n更新対象のリストアップ中...");
            var checkList = Directory.EnumerateFiles(_origFolderPath, "*", System.IO.SearchOption.AllDirectories);
            var checkListCnt = checkList.Count();

            var cnt = 0;
            int prePos = Console.CursorLeft;//現在カーソル位置を取得

            _allfiles = checkList
                .Select(file =>
                {
                    cnt++;
                    var temp = new FileData((_origFolderPath, _destinationFolderPath), file, true);
                    var per = (double)cnt / (double)checkListCnt;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(($"{GetProgressBar(20, per)}{cnt}/{checkListCnt}").PadRight(prePos));

                    return temp;
                }).Where(x => x.IsNewFile || x.IsUpdatedFile).ToList();

            Console.WriteLine($"\r\n更新対象のリストアップ完了");
            var newfilecnt = _allfiles.Count(x => x.IsNewFile);
            var updatedfilecnt = _allfiles.Count(x => x.IsUpdatedFile);
            Console.WriteLine($"\r\n更新対象は全{_allfiles.Count}件です。");
            Console.WriteLine($"更新ファイル：{updatedfilecnt}件\r\n新規ファイル：{newfilecnt}件");
            if (_allfiles.Count > 0) Duplicate();
        }

        string GetProgressBar(int barLength, double per)
        {
            var bar = (int)Math.Floor(barLength * per);
            return $"【{new string('■', bar)}{new string('□', barLength - bar)}】";
        }

        void Duplicate()
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

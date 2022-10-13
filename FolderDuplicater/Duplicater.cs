using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FolderDuplicater
{
    public class Duplicater
    {
        List<FileData> _allfiles;
        readonly string _origFolderPath;
        readonly string _destinationFolderPath;

        public Duplicater((string origPath, string destPath) target, bool isMirroring)
        {
            _origFolderPath = target.origPath;
            _destinationFolderPath = target.destPath;
            Console.WriteLine("***********************************************************");
            Console.WriteLine($"複製元のフォルダーのパス：{_origFolderPath}\r\n複製先のフォルダーのパス：{_destinationFolderPath}\r\n");
            Console.WriteLine("***********************************************************");

            try
            {
                if (!Directory.Exists(_origFolderPath) && !Directory.Exists(_destinationFolderPath))
                {
                    Console.WriteLine("複製元/複製先フォルダーが存在しません。");
                    return;
                }
                else if (!Directory.Exists(_origFolderPath))
                {
                    Console.WriteLine("複製元フォルダーが存在しません。");
                    return;
                }
                else if (!Directory.Exists(_destinationFolderPath))
                {
                    Console.WriteLine("複製先フォルダーが存在しません。");
                    return;
                }

                //複製先フォルダが有る場合に削除実行。
                if (isMirroring) DeleteNotExistFiles();
                UpdFiles();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }

        void DeleteNotExistFiles()
        {
            var checkList = Directory.EnumerateFiles(_destinationFolderPath, "*", System.IO.SearchOption.AllDirectories);
            var bar = new ProgressBar(checkList.Count());

            var delList = checkList
                .Select(file =>
                {
                    var temp = new FileData((_origFolderPath, _destinationFolderPath), file, false);
                    bar.AddCnt();
                    return temp;
                }).Where(x => x.IsDeletedFile).OrderByDescending(x => x.DestInfo.FullName.Length).ToList();
            delList.ForEach(file => file.DeleteDestFile());
        }

        void UpdFiles()
        {
            var checkList = Directory.EnumerateFiles(_origFolderPath, "*", System.IO.SearchOption.AllDirectories);
            var bar = new ProgressBar(checkList.Count());

            _allfiles = checkList
                .Select(file =>
                {
                    var temp = new FileData((_origFolderPath, _destinationFolderPath), file, true);
                    bar.AddCnt();
                    return temp;
                }).Where(x => x.IsNewFile || x.IsUpdatedFile).ToList();

            if (_allfiles.Count > 0) _allfiles.AsParallel().ForAll(file => file.DupricateFile());
        }
    }
}

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
        List<FileData> _allfiles { get; set; }
        string _origFolderPath { get; set; }
        string _destinationFolderPath { get; set; }

        public Duplicater((string origPath, string destPath) target, bool isMirroring)
        {
            _origFolderPath = target.origPath;
            _destinationFolderPath = target.destPath;

            try
            {
                if (!Directory.Exists(_origFolderPath) && !Directory.Exists(_destinationFolderPath))
                {
                    return;
                }
                else if (!Directory.Exists(_origFolderPath))
                {
                    return;
                }
                else if (!Directory.Exists(_destinationFolderPath))
                {
                    return;
                }

                //複製先フォルダが有る場合に削除実行。
                if (isMirroring) DeleteNotExistFiles();
                UpdFiles();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void DeleteNotExistFiles()
        {
            var checkList = Directory.EnumerateFiles(_destinationFolderPath, "*", System.IO.SearchOption.AllDirectories);
            var delList = checkList
                .Select(file => new FileData((_origFolderPath, _destinationFolderPath), file, false))
                .Where(x => x.IsDeletedFile).OrderByDescending(x => x.DestInfo.FullName.Length).ToList();
            delList.ForEach(file => file.DeleteDestFile());
        }

        void UpdFiles()
        {
            var checkList = Directory.EnumerateFiles(_origFolderPath, "*", System.IO.SearchOption.AllDirectories);
            _allfiles = checkList
                .Select(file => new FileData((_origFolderPath, _destinationFolderPath), file, true))
                .Where(x => x.IsNewFile || x.IsUpdatedFile).ToList();

            if (_allfiles.Count > 0) _allfiles.AsParallel().ForAll(file => file.DupricateFile());
        }
    }
}

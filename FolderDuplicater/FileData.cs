using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FolderDuplicater
{
    class FileData
    {
        public bool IsNewFile { get; set; } = true;
        public bool IsUpdatedFile { get; set; } = false;
        public FileInfo OrigInfo { get; set; }
        public string DestinationFolderPath { get; set; }
        public string DestinationFilePath { get; set; }
        public FileData((string origPath, string destPath) target, string filepath)
        {
            OrigInfo = new FileInfo(filepath);
            DestinationFolderPath = OrigInfo.DirectoryName.Replace(target.origPath, target.destPath);
            DestinationFilePath = OrigInfo.FullName.Replace(target.origPath, target.destPath);

            if (!File.Exists(DestinationFilePath)) return;
            IsNewFile = false;
            var destinationInfo = new FileInfo(DestinationFilePath);
            IsUpdatedFile = destinationInfo.LastWriteTimeUtc < OrigInfo.LastWriteTimeUtc;
        }

        public void DupricateFile()
        {
            if (!Directory.Exists(DestinationFolderPath))
                Directory.CreateDirectory(DestinationFolderPath);
            File.Copy(OrigInfo.FullName, DestinationFilePath, true);
        }
    }
}

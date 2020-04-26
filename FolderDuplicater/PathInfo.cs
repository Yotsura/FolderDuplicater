using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderDuplicater
{
    class PathInfo
    {
        public string OrigPath { get; set; }
        public string DestinationPath { get; set; }
        public PathInfo()
        {
            SetPath();
            SaveSettings();
        }

        void SaveSettings()
        {
            Settings.Default.OrigPath = OrigPath;
            Settings.Default.DestinationPath = DestinationPath;
            Settings.Default.Save();
        }
        void SetPath()
        {
            OrigPath = Settings.Default.OrigPath;
            DestinationPath = Settings.Default.DestinationPath;
            if (OrigPath != "" && DestinationPath != "")
            {
                Console.WriteLine($"複製元のフォルダーのパス：{OrigPath}");
                Console.WriteLine($"複製先のフォルダーのパス：{DestinationPath}");
                Console.WriteLine("前回の設定を使用しますか？　Y/N");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y: return;
                    case ConsoleKey.N: break;
                    default: break;
                }
            }

            for (; ; )
            {
                Console.WriteLine("複製元のフォルダーのパスを入力してください");
                OrigPath = Console.ReadLine();
                Console.WriteLine("複製先のフォルダーのパスを入力してください");
                DestinationPath = Console.ReadLine();
                Console.WriteLine($"複製元のフォルダーのパス：{OrigPath}");
                Console.WriteLine($"複製先のフォルダーのパス：{DestinationPath}");
                Console.WriteLine("この設定で実行しますか？　Y/N");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                        return;
                    case ConsoleKey.N:
                    default:
                        Console.WriteLine("\r\n再設定します。");
                        break;
                }
            }
        }
    }
}

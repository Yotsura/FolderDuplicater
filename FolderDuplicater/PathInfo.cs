using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderDuplicater
{
    class PathInfo
    {
        public List<(string Orig, string Dest)> PathPairs { get; set; }

        public PathInfo()
        {
            PathPairs = Settings.Default.PathPairs;
        }
        public void ChangeSwttings()
        {
            //if (PathPairs.Count() > 0)
            //{
            //    Console.WriteLine("***********************************************************");
            //    foreach (var (Orig, Dest) in PathPairs)
            //        Console.WriteLine($"複製元のフォルダーのパス：{Orig}\r\n複製先のフォルダーのパス：{Dest}\r\n");
            //    Console.WriteLine("***********************************************************");
            //    Console.WriteLine("このまま実行しますか？　Y/N");
            //    switch (Console.ReadKey().Key)
            //    {
            //        case ConsoleKey.Y: return;
            //        case ConsoleKey.N: break;
            //        default: break;
            //    }
            //}

            //設定追加
            for (; ; )
            {
                Console.Clear();
                Console.WriteLine("***********************************************************");
                foreach (var (Orig, Dest) in PathPairs)
                    Console.WriteLine($"複製元のフォルダーのパス：{Orig}\r\n複製先のフォルダーのパス：{Dest}\r\n");
                Console.WriteLine("***********************************************************");
                Console.WriteLine("設定変更内容を選択してください。\r\n1:設定追加\r\n2:設定の一部削除\r\n3:設定をクリアして追加\r\n" +
                    "Esc:更新画面を閉じる\r\nEnter:設定変更を保存");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        AddSettings();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        DeleteSettings();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        PathPairs = new List<(string Orig, string Dest)>();
                        AddSettings();
                        break;
                    case ConsoleKey.Escape:
                        return;
                    case ConsoleKey.Enter:
                        Settings.Default.PathPairs = PathPairs;
                        Settings.Default.Save();
                        break;
                    default: break;
                }
            }

        }

        void DeleteSettings()
        {
            var temp = PathPairs.Select(x => x).ToList();
            var idx = 0;
            for (; ; )
            {
                Console.Clear();
                Console.WriteLine("***********************************************************");
                for (var i = 0; i < temp.Count(); i++)
                {
                    var (Orig, Dest) = temp[i];
                    Console.WriteLine($"設定:{i}{(idx == i ? "＜【削除】" : "")}\r\n" +
                        $"複製元のフォルダーのパス：{Orig}\r\n複製先のフォルダーのパス：{Dest}\r\n");
                }
                Console.WriteLine("***********************************************************");
                Console.WriteLine("削除する設定を↑↓で選択し、Deleteを押してください。\r\nEnter:設定を保存/Esc:元の画面に戻る。");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        idx--;
                        if (idx < 0) idx = temp.Count() - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        idx++;
                        if (idx >= temp.Count()) idx = 0;
                        break;
                    case ConsoleKey.Delete:
                        temp.RemoveAt(idx);
                        if (idx > temp.Count - 1) idx = temp.Count() - 1;
                        continue;
                    case ConsoleKey.Escape:
                        return;
                    case ConsoleKey.Enter:
                        PathPairs = temp;
                        Settings.Default.PathPairs = PathPairs;
                        Settings.Default.Save();
                        continue;
                    default: break;
                }
            }
        }

        void AddSettings()
        {
            for (; ; )
            {
                Console.Clear();
                Console.WriteLine(("設定を追加します。\r\n複製元のフォルダーのパスを入力してください"));
                var origPath = Console.ReadLine();
                Console.WriteLine("複製先のフォルダーのパスを入力してください");
                var destinationPath = Console.ReadLine();

                Console.WriteLine("***********************************************************");
                Console.WriteLine($"複製元のフォルダーのパス：{origPath}\r\n複製先のフォルダーのパス：{destinationPath}\r\n");
                Console.WriteLine("***********************************************************");

                Console.WriteLine("この設定を追加しますか？　Y/N");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                        PathPairs.Add((origPath, destinationPath));
                        break;
                    case ConsoleKey.N:
                    default:
                        Console.WriteLine("\r\n追加設定を破棄しました。");
                        break;
                }
                Console.WriteLine("続けて設定を追加しますか？　Y/N");
                if (Console.ReadKey().Key == ConsoleKey.Y) continue;
                return;
            }
        }
    }
}

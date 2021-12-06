using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderDuplicater
{
    public class MainFunc
    {

        PathInfo targetsInfo = new PathInfo();
        public MainFunc()
        {
            var ismirroring = ModeSelect();

            Console.Clear();
            if (targetsInfo.PathPairs.Count() < 1)
            {
                Console.WriteLine("\r\n設定が存在しないため、プログラムを終了します。\r\npress any key...");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            foreach (var target in targetsInfo.PathPairs)
            {
                new Duplicater(target, ismirroring);
            }
            Console.WriteLine("\r\n＜作業完了＞\r\nプログラムを終了します。\r\npress any key...");
            Console.ReadKey();

        }

        bool ModeSelect()
        {
            for (; ; )
            {
                Console.Clear();
                Console.WriteLine("***********************************************************");
                foreach (var (Orig, Dest) in targetsInfo.PathPairs)
                    Console.WriteLine($"複製元のフォルダーのパス：{Orig}\r\n複製先のフォルダーのパス：{Dest}\r\n");
                Console.WriteLine("***********************************************************");
                Console.WriteLine("開始する作業を選択してください。\r\n1:設定の変更\r\n2:ファイルの追加・更新\r\n3:ミラーリング");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        targetsInfo.ChangeSwttings();
                        continue;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        return false;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        return true;
                    default: break;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderDuplicater
{
    class Program
    {
        static void Main(string[] args)
        {
            var targetsInfo = new PathInfo();
            if (targetsInfo.PathPairs.Count() < 1)
            {
                Console.WriteLine("\r\n設定が存在しないため、プログラムを終了します。\r\npress any key...");
                Console.ReadKey();
                return;
            }
            Console.Clear();
            Console.WriteLine("***********************************************************");
            foreach (var (Orig, Dest) in targetsInfo.PathPairs)
                Console.WriteLine($"複製元のフォルダーのパス：{Orig}\r\n複製先のフォルダーのパス：{Dest}\r\n");
            Console.WriteLine("***********************************************************");
            Console.WriteLine("開始する作業を選択してください。\r\n1:ファイルの追加・更新\r\n2:ミラーリング");

            bool ismirroring;
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    ismirroring = false;
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    ismirroring = true;
                    break;
                default: return;
            }

            Console.Clear();
            foreach (var target in targetsInfo.PathPairs)
            {
                new Duplicater(target, ismirroring);
            }
            Console.WriteLine("\r\n＜作業完了＞\r\nプログラムを終了します。\r\npress any key...");
            Console.ReadKey();
        }
    }
}

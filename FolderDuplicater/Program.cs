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
            Console.WriteLine("作業を開始するにはEnterを押してください。");
            if (Console.ReadKey().Key != ConsoleKey.Enter) return;
            Console.Clear();
            foreach (var target in targetsInfo.PathPairs)
                new Duplicater(target);

            Console.WriteLine("\r\n＜作業完了＞\r\nプログラムを終了します。\r\npress any key...");
            Console.ReadKey();
        }
    }
}

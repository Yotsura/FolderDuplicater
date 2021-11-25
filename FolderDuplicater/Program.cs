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
            var duplicater = new Duplicater(new PathInfo());
            Console.WriteLine("作業を開始するにはEnterを押してください。");
            if (Console.ReadKey().Key != ConsoleKey.Enter) return;
            duplicater.Dupticate();
            Console.ReadKey();
        }
    }
}

﻿using System;
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
            var duplicater = new Duplicater(new PathInfo());
            Console.WriteLine("作業を開始するにはEnterを押してください。");
            if (Console.ReadKey().Key != ConsoleKey.Enter) return;
            duplicater.Dupticate();
            Console.ReadKey();
        }
    }
}

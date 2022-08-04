using System;

namespace FolderDuplicater
{
    public class ProgressBar
    {
        public ProgressBar(double allCnt, int barLength, string barName)
        {
            _barLength = barLength;
            _prePos = Console.CursorLeft;
            _allCnt = allCnt;
            Console.WriteLine($"＜{barName}＞");
        }
        public ProgressBar(double allCnt, string barName)
        {
            _prePos = Console.CursorLeft;
            _allCnt = allCnt;
            Console.WriteLine($"＜{barName}＞");
        }
        public ProgressBar(double allCnt, int barLength)
        {
            _barLength = barLength;
            _prePos = Console.CursorLeft;
            _allCnt = allCnt;
        }
        public ProgressBar(double allCnt)
        {
            _prePos = Console.CursorLeft;
            _allCnt = allCnt;
        }

        readonly int _barLength = 20;
        readonly int _prePos = 0;
        readonly double _allCnt = 0;
        double _cnt = 0;
        bool IsCompleted => _cnt >= _allCnt;

        public void AddCnt()
        {
            _cnt++;
            double per = _cnt / _allCnt;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(($"{GetProgressBar(per)}{_cnt}/{_allCnt}{(per < 1 ? string.Empty : " COMPLETED\r\n")}")
                .PadRight(_prePos));
        }

        private string GetProgressBar(double per)
        {
            var bar = (int)Math.Floor(_barLength * per);
            return $"【{new string('■', bar)}{new string('□', _barLength - bar)}】";
        }
    }
}

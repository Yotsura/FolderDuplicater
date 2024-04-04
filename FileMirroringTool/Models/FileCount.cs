using System.Threading;

namespace FileMirroringTool.Models
{
    public class FileCount
    {
        int _delCnt = 0;
        int _addCnt = 0;
        int _updCnt = 0;
        int _failCnt = 0;
        public int DelCnt { get => _delCnt; set => _delCnt = value; }
        public int AddCnt { get => _addCnt; set => _addCnt = value; }
        public int UpdCnt { get => _updCnt; set => _updCnt = value; }
        public int FailCnt { get => _failCnt; set => _failCnt = value; }

        public string CntInfoStr => $"追加：{AddCnt}／更新：{UpdCnt}／更新失敗：{FailCnt}／削除：{DelCnt}";


        public void AddDelCnt() => Interlocked.Increment(ref _delCnt);
        public void AddAddCnt() => Interlocked.Increment(ref _addCnt);
        public void AddUpdCnt() => Interlocked.Increment(ref _updCnt);
        public void AddFailCnt() => Interlocked.Increment(ref _failCnt);
    }
}

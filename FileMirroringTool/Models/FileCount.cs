namespace FileMirroringTool.Models
{
    public class FileCount
    {
        public int DelCnt { get; set; } = 0;
        public int AddCnt { get; set; } = 0;
        public int UpdCnt { get; set; } = 0;
        public string CntInfoStr => $"追加：{AddCnt}／更新：{UpdCnt}／削除：{DelCnt}";
    }
}

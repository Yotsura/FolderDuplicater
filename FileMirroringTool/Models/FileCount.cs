using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMirroringTool.Models
{
    public class FileCount
    {
        public int DelCnt { get; set; } = 0;
        public int AddCnt { get; set; } = 0;
        public int UpdCnt { get; set; } = 0;

        //public void ResetCnt()
        //{
        //    DelCnt = 0;
        //    AddCnt = 0;
        //    UpdCnt = 0;
        //}

    }
}

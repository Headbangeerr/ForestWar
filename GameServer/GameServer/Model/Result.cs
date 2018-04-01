using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{ 
    /// <summary>
    /// 用户战绩
    /// </summary>
    class Result
    {
        public Result(int id,int userId,int totalCount,int winCount)
        {
            this.Id = id;
            this.UserId = userId;
            this.TotalCount = totalCount;
            this.WinCount = winCount;
        }
        public int Id { set; get; }
        public int UserId { set; get; }
        public int TotalCount { set; get; }
        public int WinCount { set; get; }
    }
}

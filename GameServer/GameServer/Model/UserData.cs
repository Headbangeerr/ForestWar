using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    class UserData
    {
        public UserData(int id,string username, int totalCount, int winCount)
        {
            this.Id = id;
            this.Username = username;
            this.TotalCount = totalCount;
            this.WinCount = winCount;
        }

        public int Id
        {
            set; get;
        }
        public string Username {  set; get; }
        public int TotalCount {  set; get; }
        public int WinCount {  set; get; }
    }
}

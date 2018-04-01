using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public User(int id,string username, string password)
        {
            this.Id = id;
            this.Username = username;
            this.Password = password;
        }     

    }
}

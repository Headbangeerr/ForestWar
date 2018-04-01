using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Servers;
using LitJson;

namespace GameServer
{
    class Start
    {
        public static void Main()
        {
            //启动服务器
            Server server = new Server("172.16.252.85", 6688);
            server.Start();
            Console.WriteLine("启动服务器……");
            //JsonData jd = new JsonData();
            //jd["result"] = 1;
            //jd["user"] = new JsonData();//**新添加一层关系时，需要再次 new** JsonData（）
            //jd["user"]["name"] = "zhang";
            //jd["user"]["password"] = 123456;
            //string jsonData = JsonMapper.ToJson(jd);

            //JsonData strToObject = JsonMapper.ToObject(jsonData);
            //int result = int.Parse(strToObject["result"].ToString());

            //Console.WriteLine(result);
            //Console.WritkeLine(jsonData);
            Console.ReadKey();
        }
    }
}

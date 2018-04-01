using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Model;
using GameServer.Servers;
using LitJson;

namespace GameServer.Controller
{
    class RoomController : BaseController
    {
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }

        /// <summary>
        /// 处理创建房间请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string CreateRoom(string data, Client client, Server server)
        {
            server.CreateRoom(client); //将创建者的Client对象传入，通过Server对象进行房间创建

            JsonData jsonData = new JsonData();
            jsonData["returnCode"] = ((int) ReturnCode.Success).ToString();
            jsonData["roleType"]= ((int)RoleType.Blue).ToString();
            //返回给客户端创建成功
            return jsonData.ToJson();
        }
        /// <summary>
        /// 处理房间列表请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string ListRoom(string data, Client client, Server server)
        {
            List<UserData> roomOwnerDataList=new List<UserData>();
            if (server.GetRoomList() != null)//如果房间个数不为0
            {
                //客户端显示房间列表只需要展示房主的个人信息即可
                foreach (Room room in server.GetRoomList())
                {
                    if (room.IsWaitingJoin())//如果房间状态为等待玩家加入，则添加至房间列表中
                    {
                        roomOwnerDataList.Add(room.GetRoomOwnerData());
                    }
                }
                return JsonMapper.ToJson(roomOwnerDataList);
            }
            else//如果当前房间数为0
            {
                return "0";
            } 
        }
        /// <summary>
        /// 处理加入房间请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string JoinRoom(string data, Client client, Server server)
        {
            int id = int.Parse(data);//请求中的数据就是要加入的房间号
            Room room=server.GetRoomById(id);//获取房间对象
            JsonData jsonData=new JsonData();
            if (room ==null)//查找失败
            {
                jsonData["returnCode"] = ((int)ReturnCode.NotFound).ToString();
            }
            else if (room.IsWaitingJoin()==false)//查找到了房间，但是房间已满，不可加入
            {
                jsonData["returnCode"] = ((int)ReturnCode.NotFound).ToString();
            }
            else//可以进入房间 
            {
                room.AddClient(client);
                string roomData = room.GetRoomData();//在这一层，已经将list集合转化为了json字符串

                jsonData["roomData"] =roomData;//将json字符串再次封装到新的json字符串中
                jsonData["returnCode"] = ((int)ReturnCode.Success).ToString();
                jsonData["roleType"] = ((int)RoleType.Red).ToString();
                //成功加入房间后，需要向房间内除了自身以外的其他玩家发送房间内的玩家信息
                room.BoradcastMessage(client,ActionCode.UpdateRoom,JsonMapper.ToJson(jsonData));
            }   
            return JsonMapper.ToJson(jsonData);
        }
        /// <summary>
        /// 处理退出房间请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        public string QuitRoom(string data, Client client, Server server)
        {
            bool isRoomOwner = client.IsRoomOwner();//首先判断该客户端是否是房主
            JsonData jsonData=new JsonData();
            Room room = client.Room;//获取客户端所在房间
            if (isRoomOwner)//如果该客户端是房主
            {
                jsonData["returnCode"] = ((int)ReturnCode.Success).ToString();//返回给自身客户端success，直接退出房间面板
                //同时给其他房间内的玩家广播发送一个quitRoom类型的响应，响应内容为success类型的returnCode，使其客户端直接退出房间面板
                room.BoradcastMessage(client,ActionCode.QuitRoom,JsonMapper.ToJson(jsonData));
                room.Close(client);
            }
            else//如果该客户端不是房主
            {                
                room.RemoveClient(client);//将客户端从所在房间移除
                string roomData = room.GetRoomData();//在这一层，已经将list集合转化为了json字符串
                jsonData["roomData"] = roomData;//将json字符串再次封装到新的json字符串中
                room.BoradcastMessage(client,ActionCode.UpdateRoom, JsonMapper.ToJson(jsonData));//向房间内玩家广播更新房间内玩家信息
                jsonData["returnCode"] = ((int)ReturnCode.Success).ToString();
            }
            return JsonMapper.ToJson(jsonData);
        }
    }
}

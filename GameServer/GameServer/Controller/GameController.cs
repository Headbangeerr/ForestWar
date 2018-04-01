using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;
using LitJson;

namespace GameServer.Controller
{
    class GameController:BaseController
    {
        public GameController()
        {
            requestCode=RequestCode.Game;
        }
        /// <summary>
        /// 处理开始游戏请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string StartGame(string data, Client client, Server server)
        {
            JsonData jsonData=new JsonData();
            if (client.IsRoomOwner())//如果客户端对象是房主
            {
                Room room = client.Room;
                jsonData["returnCode"] = ((int)ReturnCode.Success).ToString();//给自身客户端返回一个success
                //再给其他房间内的玩家返回一个success
                room.BoradcastMessage(client,ActionCode.StartGame,JsonMapper.ToJson(jsonData));
                //开始计时
                room.StartTimer();
            }
            else//如果客户端对象不是房主，则无权直接开始游戏
            {
                jsonData["returnCode"] = ((int)ReturnCode.Fail).ToString();
            }
            return JsonMapper.ToJson(jsonData);
        }
        /// <summary>
        /// 接收来自客户端的位置信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string Move(string data, Client client, Server server)
        {

            if (client.Room!=null)//
            {
                //获取该链接所在的房间对象
                Room room = client.Room;
                //对房间内除自身以外的其他客户端广播接收到的同步位置信息
                room.BoradcastMessage(client, ActionCode.Move, data);
                return null;//对于发送同步信息的客户端响应为空即可
            }
            else//房主退出房间将房间删除
            {
                return null;
            }
        }
        /// <summary>
        /// 处理来自客户端的射击请求，直接将箭矢的射击信息转发给其他玩家
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string Shoot(string data, Client client, Server server)
        {

            if (client.Room != null)//
            {
                //获取该链接所在的房间对象
                Room room = client.Room;
                //对房间内除自身以外的其他客户端广播接收到的同步位置信息
                room.BoradcastMessage(client, ActionCode.Shoot, data);
                return null;//对于发送同步信息的客户端响应为空即可
            }
            else//房主退出房间将房间删除
            {
                return null;
            }
        }
        /// <summary>
        /// 处理来自客户端造成伤害的请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string Attack(string data, Client client, Server server)
        {
            int damage = int.Parse(data);
            Room room = client.Room;//获取客户端所在房间
            if (room==null)
            {
                return null;
            }
            //对房间内除自身以外的玩家发送伤害数值
            room.TakeDamage(damage,client);
            return null;
        }
        /// <summary>
        /// 用户中途退出游戏
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string QuitBattle(string data, Client client, Server server)
        {
            Room room = client.Room;
            if (room!=null)
            {
                //给房间内所有玩家响应
                room.BoradcastMessage(null, ActionCode.QuitBattle, "");
                //关闭房间
                room.Close();
            }
            return null;
        }
    }
}

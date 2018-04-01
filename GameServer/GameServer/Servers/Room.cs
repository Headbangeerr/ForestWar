using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using GameServer.Model;
using LitJson;

namespace GameServer.Servers
{
    /// <summary>
    /// 房间状态类型
    /// </summary>
    enum RoomState
    {
        WaitingJoin,//待加入
        WaitingBattle,//房间满员，等待开始
        Battle,//正在战斗
        End//战斗结束
    }
    class Room
    {

        //用于管理房间中的客户端，一个房间最多有两个客户端连接，其中最先添加的是房主
        private List<Client> clientRoom=new List<Client>();

        private Server server;

        //房间初始化状态为“待加入”状态
        private RoomState state=RoomState.WaitingJoin;
        //血量上限
        private const int MAX_HP = 100;
        public Room(Server server)
        {
            this.server = server;
        }
        /// <summary>
        /// 返回当前房间的id
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            if (clientRoom.Count>0)
            {
                //房间id即房主的id
                return clientRoom[0].GetUserDate().Id;
            }
            return -1;
        }
        /// <summary>
        /// 向房间中添加一个客户端连接对象
        /// </summary>
        /// <param name="client"></param>
        public void AddClient(Client client)
        {
            //初始化玩家血量
            client.Hp = MAX_HP;
            clientRoom.Add(client);
            //给加入到该房间的客户端连接对象的room属性赋值
            client.Room = this;
            if (clientRoom.Count>=2)//如果房间满员，修改房间状态
            {
                state=RoomState.WaitingBattle;                
            }
        }
        /// <summary>
        /// 从房间中移除一个客户端对象
        /// </summary>
        /// <param name="client"></param>
        public void RemoveClient(Client client)
        {
            client.Room = null;//将要移除的客户端对象的房间设为空
            if (clientRoom.Count!=0)//如果房间内玩家数不为0
            {
                clientRoom.Remove(client);//从房间中移除指定客户端对象
                if (clientRoom.Count>=2)
                {
                    this.state=RoomState.WaitingBattle;
                }
                else//如果人数不足，修改房间状态为等待加入
                {
                    this.state=RoomState.WaitingJoin;
                }
            }
        }
        /// <summary>
        /// 获取房主的个人信息
        /// </summary>
        /// <returns></returns>
        public UserData GetRoomOwnerData()
        {
            return clientRoom[0].GetUserDate();
        }
        /// <summary>
        /// 判断传入的client对象是否是该房间的房主
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool IsRoomOwner(Client client)
        {
            return client == clientRoom[0];           
        }
        /// <summary>
        /// 判断该房间状态是否是等待加入
        /// </summary>
        /// <returns></returns>
        public bool IsWaitingJoin()
        {
            return this.state == RoomState.WaitingJoin;
        }
        /// <summary>
        /// 返回房间中两名玩家的个人信息
        /// </summary>
        /// <returns>以json字符串的形式返回房间玩家的个人信息，json的内容为List<UserData>对象格式</returns>
        public String GetRoomData()
        {
            List<UserData> userDataList = new List<UserData>();
            foreach (Client client in clientRoom)
            {
                userDataList.Add(client.GetUserDate());
            }
            return JsonMapper.ToJson(userDataList); 
        }
        /// <summary>
        /// 向房间内的玩家广播信息
        /// </summary>
        /// <param name="excludeClient">要排除在外的客户端</param>
        /// <param name="actionCode">客户端响应的类型</param>
        /// <param name="data">响应数据</param>
        public void BoradcastMessage(Client excludeClient,ActionCode actionCode,string data)
        {
            foreach (Client client in clientRoom)//遍历房间内的所有客户端连接
            {
                if (client!=excludeClient)//如果不是要排除的客户端连接，则通过server对象发送消息
                {
                    server.SendResponse(client,actionCode,data);
                }
            }
        }
        /// <summary>
        /// 关闭房间
        /// </summary>
        public void Close(Client client)
        {
            if (client==clientRoom[0])//只有自身是房主的情况才关闭房间，否则保留房间
            {
                foreach (Client clientTemp in clientRoom)//通过遍历，将房间内所有连接对象的room属性置空
                {
                    clientTemp.Room = null;
                }
                server.RemoveRoom(this);
            }
            else//如果自身不是房主，则只需将自身的client对象从房间中移除即可
            {
                clientRoom.Remove(client);
            }    
        }
        /// <summary>
        /// 开启一个线程，开始计时
        /// </summary>
        public void StartTimer()
        {
            new Thread(RunTimer).Start();
        }
        /// <summary>
        /// 在线程中进行倒计时
        /// </summary>
        private void RunTimer()
        {
            Thread.Sleep(1000);//先休眠1秒，用于各个客户端准备
            for (int i = 3; i >0; i--)
            {
                //每隔一秒向房间内所有玩家广播一次倒计时
                BoradcastMessage(null,ActionCode.ShowTimeer,i.ToString());
                Thread.Sleep(1000);//线程休眠一秒
            }
            //计时结束以后，再次广播，通知所有玩家开始游戏
            BoradcastMessage(null,ActionCode.StartPlay, "startGame");
        }
        /// <summary>
        /// 造成伤害
        /// </summary>
        /// <param name="damage">伤害数值</param>
        /// <param name="excludeClient">造成伤害的一方的客户端连接</param>
        public void TakeDamage(int damage, Client excludeClient)
        {
           
            bool isDead=false;
            foreach (Client client in clientRoom)
            {
                if (client!=excludeClient)
                {
                    //减少hp，判断是否死亡
                    isDead =client.TakeDamage(damage);
                    Console.WriteLine("收到来自[" + excludeClient.GetUserDate().Username + "]的伤害，伤害值为：" + damage+",当前HP："+client.Hp);
                }
            }
            //如果其中一个角色死亡，要结束游戏
            if (isDead)
            {
                JsonData jsonData = new JsonData();
                //遍历所有客户端
                foreach (Client client in clientRoom)
                {
                    //如果该客户端死亡
                    if (client.isDead())
                    {
                        Console.WriteLine("玩家["+client.GetUserDate().Username+"]被击杀");
                        jsonData["returnCode"] = ((int)ReturnCode.Fail).ToString();
                        //向该客户端发送游戏结束的响应，对应的游戏结果为失败
                        client.Send(ActionCode.GameOver,JsonMapper.ToJson(jsonData));
                        //玩家失败，更新战绩信息
                        client.UpdateResult(false);
                    }
                    else//该客户端角色没有死亡
                    {
                        jsonData["returnCode"] = ((int)ReturnCode.Success).ToString();
                        //向该客户端发送游戏结束的响应，对应的游戏结果为成功
                        client.Send(ActionCode.GameOver, JsonMapper.ToJson(jsonData));
                        //玩家胜利，更新战绩
                        client.UpdateResult(true);
                    }
                }
                Close();
            }
        }
        /// <summary>
        /// 关闭房间
        /// </summary>
        public void Close()
        {
            foreach (Client client in clientRoom)
            {
                client.Room = null;
            }
            server.RemoveRoom(this);
        }
    }
}

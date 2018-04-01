using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.DAO;
using GameServer.Model;
using GameServer.Tool;
using LitJson;
using MySql.Data.MySqlClient;

namespace GameServer.Servers
{
    class Client
    {
        private Socket clientSocket;//客户端连接对象
        private Server server;//服务器对象
        private Message msg=new Message();//数据处理对象
        private MySqlConnection mysqlConn;//数据库连接对象
        //用于存储该用户的个人信息以及战绩信息，在登录成功以后就进行保存
        private UserData userData;

        private ResultDAO resultDao=new ResultDAO();
        private Room room;//该连接所在游戏房间
        

        public int Hp { get;set; }

        /// <summary>
        /// 保存该客户端的登录用户的个人数据
        /// </summary>
        /// <param name="userData"></param>       
        public void SetUserData(UserData userData)
        {
            this.userData = userData;
        }
        /// <summary>
        /// 获取用户的个人数据
        /// </summary>
        /// <returns>返回该连接对象的登陆者信息，以json字符串格式保存</returns>
        public UserData GetUserDate()
        {
            return userData;
        }

        public Room Room
        {
            set { room = value; }
            get { return room; }
        }

        public Client()
        {
        }

        public MySqlConnection MySqlConnection
        {
            get { return mysqlConn; }
        }
        /// <summary>
        /// 带参构造方法
        /// </summary>
        /// <param name="clientSocket">已连接的客户端连接对象</param>
        /// <param name="server">服务器对象</param>
        public Client(Socket clientSocket,Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            this.mysqlConn = ConnHelper.Connect();//开启一个数据库连接，一个客户端连接对应一个数据库连接
        }
        /// <summary>
        /// 开始准备接收数据
        /// </summary>
        public void Start()
        {
            //如果连接被远程客户端前置关闭连接，则直接返回，不再进行接收，防止抛出异常
            if (clientSocket == null || clientSocket.Connected == false)
            {
                Console.WriteLine("远程客户端强制断开连接……");
                Close();
                return;
            }
            //开始准备接收数据，将数据存取Message对象用于后续的解析处理
            clientSocket.BeginReceive(msg.Data,msg.StartIndex,msg.ResidueSize, SocketFlags.None, ReceiveCallBack,null);
        }
        /// <summary>
        /// ReadMessage方法的回调方法，由于Message类只负责请求的解析，为了避免在其中出现过多的业务逻辑处理，需要使用回调函数将解析的结果传出
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="actionCode"></param>
        /// <param name="data"></param>
        private void OnProcessMessage(RequestCode requestCode, ActionCode actionCode, string data)
        {
            server.HandleRequest(requestCode,actionCode,data,this);//通过服务器对象进行请求处理
        }
        /// <summary>
        /// 成功接收数据后的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try//用于处理客户端端强制断开连接，防止抛出异常
            {
                if (clientSocket == null || clientSocket.Connected == false)
                {
                    Console.WriteLine("远程客户端强制断开连接……");
                    Close();
                    return;
                }
                int dataLength = clientSocket.EndReceive(ar);//返回成功获取的数据长度
                if (dataLength == 0)//如果数据长度为0，则代表客户端已经主动断开连接
                {
                    Close();//关闭连接                 
                }                
                msg.ReadMessage(dataLength,OnProcessMessage);//处理接收到的数据
                //clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.ResidueSize, SocketFlags.None, ReceiveCallBack, null);
                Start();//再次开始监听，准备接收数据，形成循环调用
            }
            catch (Exception e)
            {
                Console.WriteLine(e);//输出异常至控制台
                Console.WriteLine("远程客户端强制断开连接……");
                Close();//关闭连接
            }           
        }
        /// <summary>
        /// 向客户端发送数据
        /// </summary>
        /// <param name="actionCode">客户端要响应的类型</param>
        /// <param name="data"></param>
        public void Send(ActionCode actionCode,string data)
        {
            try
            {
                byte[] package = Message.PackData(actionCode, data);//包装数据
                clientSocket.Send(package);//想客户端连接发送数据
            }
            catch (Exception e)
            {
                Console.WriteLine("无法发送消息："+e);                
            }
        }
        /// <summary>
        /// 判断自身是否为所在房间的房主
        /// </summary>
        /// <returns></returns>
        public bool IsRoomOwner()
        {
            return this.room.IsRoomOwner(this);
        }
        /// <summary>
        /// 承受伤害，降低自身HP
        /// </summary>
        /// <param name="damage"></param>
        /// <returns>是否死亡</returns>
        public bool TakeDamage(int damage)
        {
            this.Hp -= damage;
            //hp不能小于0
            this.Hp = Math.Max(Hp, 0);
            //hp等于0时，代表角色死亡，游戏结束
            if (Hp <= 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 该客户端的角色是否死亡
        /// </summary>
        /// <returns></returns>
        public bool isDead()
        {
            return this.Hp <= 0;
        }
        /// <summary>
        /// 关闭与客户端的连接
        /// </summary>
        private void Close()
        {
            ConnHelper.CloseConnection(this.mysqlConn);//关闭数据库连接
            if (clientSocket!=null)
            {
                clientSocket.Close();                
            }
            /*关闭自身所在的房间，必须先关闭房间，再移除Client，否则可能先移除client以后，再关闭room过程中
             * 无法获取到已被删除的client对象，会造成关闭房间失败
             */
            if (room != null)
            {
                room.Close(this);
            }
            server.RemoveClient(this);//通过server对象将自身从管理列表中移除            
        }
        /// <summary>
        /// 更新用户的战绩
        /// </summary>
        /// <param name="isVictory">用户是否胜利</param>
        public void UpdateResult(bool isVictory)
        {

            UpdateResultToDB(isVictory);
            //更新到客户端
            UpdateResultToClient();
        }
        /// <summary>
        /// 更新数据到数据库
        /// </summary>
        /// <param name="isVictory"></param>
        public void UpdateResultToDB(bool isVictory)
        {           
            userData.TotalCount++;
            if (isVictory)
            {
                userData.WinCount++;
            }
            //更新数据库数据
            resultDao.UpdateResult(this.mysqlConn, userData);
        }
        /// <summary>
        /// 更新数据到客户端，
        /// </summary>
        public void UpdateResultToClient()
        {     
            //通过数据获取当前用户最新的战绩信息
            Result result=resultDao.GetResultByUserId(this.mysqlConn,userData.Id);
            JsonData jsonData = new JsonData();
            jsonData["totalCount"] = result.TotalCount;
            jsonData["winCount"] = result.WinCount;
            //将获取到的result对象发送到客户端
            Send(ActionCode.UpdateResult,JsonMapper.ToJson(jsonData) );
        }
    }
}

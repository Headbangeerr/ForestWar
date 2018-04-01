using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Common;
using GameServer.Controller;

namespace GameServer.Servers
{
    class Server
    {
        private IPEndPoint ipEndPoint;//端口号
        private Socket serverSocket;//服务器端socket对象
        private List<Client> clientList;//用于管理已连接的所有client的List
        private List<Room> roomList;//用于管理游戏房间
        private ControllerManager controllerManager;//为了避免Client对象直接是用ControllerManager，同时也是为了降低耦合性
        public List<Room> GetRoomList()
        {
            return roomList;
        }

        public Server()
        {
        }

        public Server(string ipStr, int port)
        {
            controllerManager=new ControllerManager(this);//将自身的Server对象传给controllerManager
            SetIpEndPort(ipStr,port);
        }

        /// <summary>
        /// 设置服务器端口号与ip地址
        /// </summary>
        /// <param name="ipStr">字符类型ip地址</param>
        /// <param name="port">端口号</param>
        public void SetIpEndPort(string ipStr, int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
        }
        /// <summary>
        /// 启动服务器端
        /// </summary>
        public void Start()
        {
            serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);//创建服务器端socket对象
            serverSocket.Bind(ipEndPoint);//绑定ip地址与端口号
            serverSocket.Listen(0);//开始监听，并且设置等待队列长度为无限大
            serverSocket.BeginAccept(AcceptCallBack, null);//开始准备接受客户端连接
            clientList=new List<Client>();
        }
        /// <summary>
        /// 服务器端接受客户端连接后的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);//通过服务器端socket对象获取与客户端的连接
            Client client=new Client(clientSocket,this);//通过客户端连接对象新建Client     
            Console.WriteLine("成功连入一个客户端！");
            client.Start();//开始监听，准备接收数据
            clientList.Add(client);
            serverSocket.BeginAccept(AcceptCallBack, null);//再次准备接收其他客户端的连接
        }


        /// <summary>
        /// 向客户端发送请求的处理结果
        /// </summary>
        /// <param name="client">要发送的客户端对象</param>
        /// <param name="actionCode">请求编号</param>
        /// <param name="data">要发送的响应数据</param>
        public void SendResponse(Client client, ActionCode actionCode, string data)
        {
            client.Send(actionCode,data);//通过客户端连接对象发送响应数据
        }


        /// <summary>
        /// 处理客户端请求
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="actionCode"></param>
        /// <param name="data"></param>
        /// <param name="client"></param>
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, Client client)
        {
            controllerManager.HandleRequest(requestCode,actionCode,data,client);
        }


        /// <summary>
        /// 移除已经被关闭的客户端连接对象
        /// </summary>
        /// <param name="client">要移除的客户端对象</param>
        public void RemoveClient(Client client)
        {
            lock (clientList)//线程的同步机制：加锁。由于操作过程中会出现多线程异步操作，所以需要使用lock关键字进行资源加锁，防止出现同时操作
            {
                clientList.Remove(client);
            }
        }
        /// <summary>
        /// 创建一个游戏房间
        /// </summary>
        /// <param name="client">房主的Client对象</param>
        public void CreateRoom(Client client)
        {
            Room room = new Room(this);
            room.AddClient(client);//将房主的连接对象先添加到房间中
            //如果是第一次添加房间，则先实例化roomList
            if (roomList==null)
            {
                roomList=new List<Room>();
            }
            roomList.Add(room);//将刚刚创建的Room添加到管理列表中
        }
        /// <summary>
        /// 移除房间
        /// </summary>
        /// <param name="room"></param>
        public void RemoveRoom(Room room)
        {
            if (roomList!=null&&room!=null)
            {
                roomList.Remove(room);
            }
        }
        /// <summary>
        /// 通过房间id获取到房间对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Room GetRoomById(int id )
        {
            //Room room = roomList.Find(delegate (Room temp)
            //  {
            //      return temp.GetId() == id;
            //  });


            //利用find方法并通过lambda表达式获取对应id的room对象
            Room room = roomList.Find(temp => temp.GetId() == id);
            return room;
        }     
    }
}

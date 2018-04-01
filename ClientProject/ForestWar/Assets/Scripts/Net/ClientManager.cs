using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Common;
using UnityEngine;

public class ClientManager:BaseManager
{
    private const string IP = "39.107.101.85";//服务器端IP地址
    private const int PORT = 6688;//服务器端端口号
    private Socket clientSocket;//与服务器端的连接
    private Message msg=new Message();//用于数据处理的Message对象


    public ClientManager(GameFacade facade) : base(facade)
    {
    }
    /// <summary>
    /// 初始化方法
    /// </summary>
    public override void OnInit()
    {
        base.OnInit();
         clientSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        try//尝试连接到服务器
        {
            clientSocket.Connect(IP, PORT);//连接服务器
            Start();//开始监听，准备接收来自服务器端的数据
            Debug.LogWarning("成功连接至服务器！");
        }
        catch (Exception e)
        {
           Debug.LogWarning("无法连接到服务器！");
           Debug.LogWarning(e);
        }        
    }
    /// <summary>
    /// 开始接收来自服务器端的数据
    /// </summary>
    private void Start()
    {
        clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.ResidueSize, SocketFlags.None,ReceiveCallBack,null);        
    }
    /// <summary>
    /// 接收数据回调函数
    /// </summary>
    private void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            if (clientSocket==null||clientSocket.Connected==false) return;           
            int dataLenght = clientSocket.EndReceive(ar);//接收到的数据长度
            msg.ReadMessage(dataLenght, OnProcessDataCallBack);//读取接收到的数据，并将解析出来的数据交由回调函数进一步处理
            Start();//再次开始监听，准备接收数据
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }        
    }
    /// <summary>
    /// 对成功解析的数据做具体处理的回调函数
    /// </summary>
    private void OnProcessDataCallBack(ActionCode actionCode,string data)
    {       
        GameFacade.Instance.HandleResponse(actionCode,data);//通过GameFacade来调用RequestManager来处理响应消息
    }
    /// <summary>
    /// 向服务器端发送请求
    /// </summary>
    /// <param name="requestCode">请求编号</param>
    /// <param name="actionCode">处理方法编号</param>
    /// <param name="data">请求数据</param>
    public void SendRequest(RequestCode requestCode,ActionCode actionCode,string data)
    {
        byte[] dataBytes = Message.PackData(requestCode, actionCode, data);//通过Message类进行数据包装
        clientSocket.Send(dataBytes);//发送请求数据包
    }


    /// <summary>
    /// 销毁方法
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
        try
        {
            clientSocket.Close();//关闭与服务器端的连接
        }
        catch (Exception e)
        {
           Debug.LogWarning("关闭连接失败！");
        }
    }

}

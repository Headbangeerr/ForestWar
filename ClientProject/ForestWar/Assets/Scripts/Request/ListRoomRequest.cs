using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class ListRoomRequest : BaseRequest
{

    private RoomListPanel roomListPanel;
    public override void Awake()
    {
        this.requestCode=RequestCode.Room;
        this.actionCode=ActionCode.ListRoom;
        roomListPanel= GetComponent<RoomListPanel>();


        base.Awake();
    }
    
    public override void SendRequest()
    {
        //不需要想服务器端发送任何参数
        base.SendRequest("listRoom");
    }
    /// <summary>
    /// 获取服务器端返回的房间列表
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        List<UserData> roomList = new List<UserData>();
        if (data != "0")//房间数不为零，开始解析json字符串中的list集合
        {
            roomList = JsonMapper.ToObject<List<UserData>>(data);
        }      
        //通过roomListPanel中的异步方法控制UI界面加载房间信息列表
        roomListPanel.LoadRoomListSync(roomList);
    }
}

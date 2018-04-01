using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class UpdateRoomRequest : BaseRequest
{
    private RoomPanel roomPanel;
    public override void Awake()
    {
        roomPanel = GetComponent<RoomPanel>();

        this.requestCode=RequestCode.Room;
        this.actionCode = ActionCode.UpdateRoom;
        base.Awake();
    }
    /// <summary>
    /// 处理房间进入其他玩家后的响应
    /// </summary>
    /// <param name="data">服务器端返回的数据是房间内所有玩家的信息</param>
    public override void OnResponse(string data)
    {
        UserData ud1 = null;
        UserData ud2 = null;
        JsonData jsonData = JsonMapper.ToObject(data);
        string roomDataJson = jsonData["roomData"].ToString();
        List<UserData> roomData = JsonMapper.ToObject<List<UserData>>(roomDataJson);//根据上面得到的json字符串在解析成对应的list集合
        if (roomData.Count==2)//如果房间内是两名玩家的话，代表是刚刚新加入了玩家，需要更新两名玩家的信息
        {
            Debug.Log("有玩家进入房间了");
            ud1 = roomData[0];//第一个为房主信息
            ud2 = roomData[1];//第二个为另一个玩家信息
            roomPanel.SetRoomDataSync(ud1, ud2);//异步更新房间内的玩家信息
        }
        else if (roomData.Count==1)//如果房间内是一名玩家的话，代表刚刚退出了一名玩家，需要将退出的玩家的信息清空
        {
            Debug.Log("我是房主：刚刚有玩家退出了");
            ud1 = roomData[0];//第一个为房主信息
            roomPanel.SetRoomDataSync(ud1,ud2);
        }               
    }

    void Update () {
		
	}
}

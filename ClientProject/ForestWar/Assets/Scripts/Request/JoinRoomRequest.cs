using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class JoinRoomRequest : BaseRequest
{

    private RoomListPanel roomListPanel;
    public override void Awake()
    {
        this.requestCode=RequestCode.Room;
        this.actionCode=ActionCode.JoinRoom;
        roomListPanel = GetComponent<RoomListPanel>();
        base.Awake();
    }
    /// <summary>
    /// 发送加入房间请求
    /// </summary>
    /// <param name="id">要进入的房间号</param>
    public void SendRequest(int id)
    { 
        base.SendRequest(id.ToString());
    }
    /// <summary>
    /// 响应服务器端响应
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        JsonData jsonData = JsonMapper.ToObject(data);
        ReturnCode returnCode = (ReturnCode)int.Parse(jsonData["returnCode"].ToString());//获取到返回码
        RoleType roleType;
        UserData ud1 = null;
        UserData ud2 = null;
        if (returnCode == ReturnCode.Success)//只有当成功加入房间时才可以取到房间中的玩家信息
        {
            string roomDataJson = jsonData["roomData"].ToString();//这roomData对应的是一个Json字符串，里面存储的是list集合数据
            List<UserData> roomData = JsonMapper.ToObject<List<UserData>>(roomDataJson);//根据上面得到的json字符串在解析成对应的list集合
            ud1 = roomData[0];//第一个为房主信息
            ud2 = roomData[1];//第二个为另一个玩家信息
            //成功加入后，才能获取到自身的角色类型
            roleType = (RoleType)int.Parse(jsonData["roleType"].ToString());
            gameFacade.SetCurrentRoleType(roleType);//设置自身的角色类型
        }
        //ui面板对响应信息作相应处理
        roomListPanel.OnJoinResponse(returnCode,ud1,ud2);
        
    }
}

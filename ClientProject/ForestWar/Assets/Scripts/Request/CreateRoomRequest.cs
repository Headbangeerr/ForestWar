using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class CreateRoomRequest : BaseRequest
{
    private RoomPanel roomPanel;
    /// <summary>
    /// 用于注入RoomPanel实例
    /// </summary>
    /// <param name="panel"></param>
    public void SetRoomPanel(BasePanel panel)
    {        
        this.roomPanel = panel as RoomPanel;         
    }
    public override void Awake()
    {
        //获取UI控制脚本对象
        roomPanel = GetComponent<RoomPanel>();
       
        this.requestCode=RequestCode.Room;
        this.actionCode =ActionCode.CreateRoom;
        base.Awake();
    }
    /// <summary>
    /// 发送创建房间请求
    /// </summary>
    public override void SendRequest()
    {
        //直接通过父类方法，想后台发送请求，这里本不需要任何参数，只需要发送RequestCode与ActionCode即可
        base.SendRequest("create");
    }
    /// <summary>
    /// 接收到响应以后的回调方法
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        //解析Json字符串
        JsonData jsonData = JsonMapper.ToObject(data);
        ReturnCode returnCode = (ReturnCode)int.Parse(jsonData["returnCode"].ToString());
        //获取角色类型，如果是房主则值为blue，一般玩家为red
        RoleType roleType= (RoleType)int.Parse(jsonData["roleType"].ToString());        
        gameFacade.SetCurrentRoleType(roleType);//设置角色类型
        if (returnCode==ReturnCode.Success)
        {                        
            //异步显示房主自己的个人信息，同时清空另一名玩家的信息
            roomPanel.SetBluePlayerInfoSync();
        }
    }
}

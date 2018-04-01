using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class QuitRoomRequest : BaseRequest
{

    private RoomPanel roomPanel;
    public override void Awake()
    {
        roomPanel = GetComponent<RoomPanel>();
        this.requestCode = RequestCode.Room;
        this.actionCode = ActionCode.QuitRoom;
        base.Awake();
    }
    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 发送退出房间请求
    /// </summary>
    public override void SendRequest()
    {
        base.SendRequest("quitRoom");
    }
    /// <summary>
    /// 处理
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        JsonData jsonData = JsonMapper.ToObject(data);
        ReturnCode returnCode = (ReturnCode)int.Parse(jsonData["returnCode"].ToString());//获取到返回码
        if (returnCode==ReturnCode.Success)//如果退出成功
        {
            roomPanel.OnQuitRoomResponse();
        }
    }

   
}

using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class StartGameRquest : BaseRequest
{
    private RoomPanel roomPanel;
    public override void Awake()
    {
        this.requestCode=RequestCode.Game;
        this.actionCode=ActionCode.StartGame;
        roomPanel = GetComponent<RoomPanel>();
        Debug.Log("actionCode:"+actionCode);
        base.Awake();
    }
    /// <summary>
    /// 无需参数，直接发送开始游戏请求即可
    /// </summary>
    public override void SendRequest()
    {
        base.SendRequest("startGame");
    }

    public override void OnResponse(string data)
    {
        JsonData jsonData = JsonMapper.ToObject(data);//解析json字符串
        ReturnCode returnCode = (ReturnCode)int.Parse(jsonData["returnCode"].ToString());
        roomPanel.OnStartGameResponse(returnCode);
    }

// Update is called once per frame
	void Update () {
		
	}
}

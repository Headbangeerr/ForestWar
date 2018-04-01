using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class GameOverRequest : BaseRequest
{
    private GamePanel gamePanel;
    private bool isGameOver;
    private ReturnCode returnCode;
    public override void Awake()
    {
        gamePanel = GetComponent<GamePanel>();
        this.requestCode=RequestCode.Game;
        this.actionCode=ActionCode.GameOver;
        base.Awake();
    }

    private void Update()
    {
        if (isGameOver)//异步处理游戏结束
        {
            //游戏面板处理游戏结果 
            gamePanel.OnGameOverResponse(returnCode);
            isGameOver = false;
        }
    }
    /// <summary>
    /// 处理游戏结束响应
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        //解析游戏结果
        JsonData jsonData = JsonMapper.ToObject(data);
        returnCode = (ReturnCode)int.Parse(jsonData["returnCode"].ToString());       
       
        isGameOver = true;
    }
}

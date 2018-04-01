using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class StartPlayRequest : BaseRequest
{
    private bool isStartPlaying = false;
    public override void Awake()
    {
        this.requestCode=RequestCode.Game;
        this.actionCode=ActionCode.StartPlay;
        Debug.Log("actionCode:"+actionCode);
        base.Awake();
    }

    private void Update()
    {
        if (isStartPlaying)
        {
            //角色添加控制脚本
            gameFacade.StartPlaying();
            isStartPlaying = false;
        }
    }
    /// <summary>
    /// 接收开始游戏的响应
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        //接收到来自服务器端的开始游戏的响应，给角色添加控制脚本
        isStartPlaying = true;
    }
}

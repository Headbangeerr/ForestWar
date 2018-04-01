using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class ShowTimerRequest : BaseRequest
{
    private GamePanel gamePanel;
    public override void Awake()
    {
        this.requestCode=RequestCode.Game;
        this.actionCode=ActionCode.ShowTimeer;
        this.gamePanel = GetComponent<GamePanel>();
        base.Awake();
    }

    public override void OnResponse(string data)
    {
       
        //获取当前倒计时的时间
        int time = int.Parse(data);
        gamePanel.ShowTimeSync(time);//异步修改时间显示 
    }

// Update is called once per frame
	void Update () {
		
	}
}

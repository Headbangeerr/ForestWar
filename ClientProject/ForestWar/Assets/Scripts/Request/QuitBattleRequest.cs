using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class QuitBattleRequest : BaseRequest
{
    private GamePanel gamePanel;
    private bool isQuitBattle = false;
    public override void Awake()
    {
        this.gamePanel = GetComponent<GamePanel>();
        this.requestCode=RequestCode.Game;
        this.actionCode=ActionCode.QuitBattle;
        base.Awake();
    }

    private void Update()
    {
        if (isQuitBattle)
        {
            gamePanel.OnQuitResponse();
            isQuitBattle = false;
        }
    }

    public override void SendRequest()
    {
        base.SendRequest("quitGame");
    }

    public override void OnResponse(string data)
    {
        isQuitBattle = true;
    }
}

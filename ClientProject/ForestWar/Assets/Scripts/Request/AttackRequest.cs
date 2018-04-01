using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class AttackRequest : BaseRequest
{
   
    public override void Awake()
    {
        this.requestCode=RequestCode.Game;
        this.actionCode=ActionCode.Attack;
        
        base.Awake();
    }
    /// <summary>
    /// 发送造成伤害的请求
    /// </summary>
    /// <param name="damage"></param>
    public void SendRequest(int damage)
    {
        Debug.Log("对敌方造成了伤害");
        base.SendRequest(damage.ToString());
    }
}

using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;
/// <summary>
/// 用于同步箭矢移动的请求对象
/// </summary>
public class ShootRequest : BaseRequest
{
    public PlayerManager playerManager;
    //用于异步生成箭矢
    private Vector3 arrowPos;
    private Vector3 arrowRotation;
    private RoleType roleType;
    private bool isShoot;
    public override void Awake()
    {
        this.requestCode=RequestCode.Game;
        this.actionCode=ActionCode.Shoot;
        base.Awake();
    }

    private void Update()
    {
        if (isShoot)
        {
            playerManager.RemoteShoot(roleType,arrowPos,arrowRotation);
            isShoot = false;
        }
    }
    /// <summary>
    /// 发送射击箭矢的请求
    /// </summary>
    /// <param name="roleType">发送自身的角色类型，使得其他客户端判断发射的箭矢颜色</param>
    /// <param name="pos">发射的位置</param>
    /// <param name="rotation">发射的方向</param>
    public void SendRequest(RoleType roleType,Vector3 pos,Vector3 rotation)
    {
        //将发射箭矢的位置信息发送给服务器端
        JsonData jsonData=new JsonData();
        jsonData["roleType"] = ((int)roleType).ToString();

        jsonData["pos"]=new JsonData();
        jsonData["pos"]["x"] = pos.x;
        jsonData["pos"]["y"] = pos.y;
        jsonData["pos"]["z"] = pos.z;
        jsonData["rotation"]=new JsonData();
        jsonData["rotation"]["x"] = rotation.x;
        jsonData["rotation"]["y"] = rotation.y;
        jsonData["rotation"]["z"] = rotation.z;        
        base.SendRequest(JsonMapper.ToJson(jsonData));
        Debug.Log("SendShoot:"+ JsonMapper.ToJson(jsonData));
    }

    public override void OnResponse(string data)
    {
        Debug.Log("shootResponse:"+data);
        //解析服务器端返回的位置信息       
        JsonData jsonData = JsonMapper.ToObject(data);
        arrowPos.x = float.Parse(jsonData["pos"]["x"].ToString());
        arrowPos.y = float.Parse(jsonData["pos"]["y"].ToString());
        arrowPos.z = float.Parse(jsonData["pos"]["z"].ToString());
        Debug.Log("pos"+arrowPos);
        arrowRotation.x = float.Parse(jsonData["rotation"]["x"].ToString());
        arrowRotation.y = float.Parse(jsonData["rotation"]["y"].ToString());
        arrowRotation.z = float.Parse(jsonData["rotation"]["z"].ToString());
        Debug.Log("rotation:"+arrowRotation);
        roleType = (RoleType) int.Parse(jsonData["roleType"].ToString());
        isShoot = true;
    }
}

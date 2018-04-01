using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;
/// <summary>
/// 用于处理角色移动同步的请求对象
/// </summary>
public class MoveRequest : BaseRequest
{
    /*                     注意
     *  下面的本地角色指的是客户端正在操作的角色，而远程角色指的是另外一名玩家控制的角色，
     *  本地角色可通过本地的PlayerManager对象进行获取，而远程角色的位置信息同时通过服务器端接收得到的
     */
    //本地角色的位置信息
    private Transform localPlayerTransform;
    //本地角色的移动脚本，其中的forward属性可以知道角色是否在移动，如果移动了则需要播放动画
    private PlayerMove localPlayerMove;

    //远程角色对象
    private Transform remotePlayerTransform;
    //远程角色对象的动画组件
    private Animator remoteAnimator;
    //同步敌对角色的位置信息
    private bool isSyncRemotePlayer = false;
    //用于更新的敌人角色的信息
    private Vector3 enemyPos;
    private Vector3 enemyRotation;
    private float enemyForward;
    private int syncRate = 50;//同步频率，每秒钟同步30次

    public override void Awake()
    {
        this.requestCode=RequestCode.Game;
        this.actionCode=ActionCode.Move;        
        base.Awake();
    }

    private void Start()
    {
        //重复调用同步方法，从第0秒开始，重复频率为1/30，即一秒30次
        InvokeRepeating("SyncLocalPlayer",0,1f/syncRate);
    }

    private void FixedUpdate()
    {
        if (isSyncRemotePlayer)
        {
            SyncRemotePlayer();
            isSyncRemotePlayer = false;
        }
    }
    /// <summary>
    /// 设置本地角色位置信息的接口
    /// </summary>
    /// <param name="loacalPlayerTransform"></param>
    /// <param name="localPlayerMove"></param>
    public void SetLocalPlayer(Transform loacalPlayerTransform, PlayerMove localPlayerMove)
    {
        this.localPlayerTransform = loacalPlayerTransform;
        this.localPlayerMove = localPlayerMove;
    }
    //设置远程敌对角色的Transform组件
    public void SetRemotePlayer(Transform remotePlayerTransform)
    {
        this.remotePlayerTransform = remotePlayerTransform;
        //通过Transform组件获取动画组件
        this.remoteAnimator = this.remotePlayerTransform.GetComponent<Animator>();
    }
    /// <summary>
    /// 向其他房间内的客户端发送同步本地角色信息
    /// </summary>
    private void SyncLocalPlayer()
    {
        SendRequest(localPlayerTransform.position,localPlayerTransform.eulerAngles,localPlayerMove.forward);
    }
    /// <summary>
    /// 接收到来自服务器端广播的敌对玩家的位置信息，直接修改敌对角色的位置
    /// </summary>
    private void SyncRemotePlayer()
    {
        //接收到来自服务器的敌对角色的位置信息，控制敌对角色的移动
        remotePlayerTransform.position = enemyPos;
        remotePlayerTransform.eulerAngles = enemyRotation;
        remoteAnimator.SetFloat("Forward",enemyForward);
    }

    /// <summary>
    /// 向服务器端发送同步信息
    /// </summary>
    /// <param name="pos">位置信息</param>
    /// <param name="rotation">角色欧拉角信息</param>
    /// <param name="forward">移动动画参数</param>
    private void SendRequest(Vector3 pos, Vector3 rotation, float forward)
    {        
        //将要同步的各种位置信息拼接成json字符串
        JsonData requestData=new JsonData();
        requestData["pos"]=new JsonData();
        requestData["pos"]["x"] = pos.x;
        requestData["pos"]["y"] = pos.y;
        requestData["pos"]["z"] = pos.z;
        requestData["rotation"]=new JsonData();
        requestData["rotation"]["x"] = rotation.x;
        requestData["rotation"]["y"] = rotation.y;
        requestData["rotation"]["z"] = rotation.z;
        requestData["forward"] = forward;
        base.SendRequest(JsonMapper.ToJson(requestData));
    }

    public override void OnResponse(string data)
    {
        //解析服务器端返回的位置信息       
        JsonData jsonData = JsonMapper.ToObject(data);
        enemyPos.x = float.Parse(jsonData["pos"]["x"].ToString());
        enemyPos.y = float.Parse(jsonData["pos"]["y"].ToString());
        enemyPos.z = float.Parse(jsonData["pos"]["z"].ToString());

        enemyRotation.x = float.Parse(jsonData["rotation"]["x"].ToString());
        enemyRotation.y = float.Parse(jsonData["rotation"]["y"].ToString());
        enemyRotation.z = float.Parse(jsonData["rotation"]["z"].ToString());

        enemyForward = float.Parse(jsonData["forward"].ToString());
        isSyncRemotePlayer = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class PlayerManager : BaseManager
{

    private UserData userData;//当前客户端登陆的用户信息

    //用于管理游戏人物信息的字典对象，用于游戏中动态生成
    private Dictionary<RoleType,RoleData> roleDataDict=new Dictionary<RoleType, RoleData>();    
    private Transform rolePositions;//游戏人物出生位置   
    private RoleType currentRoleType;//当前客户端要操作的角色类型    
    private GameObject currentRoleGo;//当前客户端操作的角色的游戏物体
    private GameObject playerSyncRequest;//用于管理发起同步请求的物体，将发起同步请求的脚本都挂在到这个物体上
    private GameObject remoteRoleGo;//敌对角色的游戏物体
    private ShootRequest shootRequest;//同步箭矢动态的请求对象
    private AttackRequest attackRequest;//造成伤害的请求对象
    /// <summary>
    /// 设置当前客户端的角色类型
    /// </summary>
    /// <param name="roleType"></param>
    public void SetCurrentRoleType(RoleType roleType)
    {
        this.currentRoleType = roleType;
    }
    /// <summary>
    /// 获取当前客户端要操作的游戏角色物体
    /// </summary>
    /// <returns></returns>
    public GameObject GetCurrentRoleGo()
    {
        return currentRoleGo;
    }
    public UserData UserData
    {
        set { userData = value; }
        get { return userData; }
    }

    public override void OnInit()
    {
        //获取出生位置
        rolePositions = GameObject.Find("RolePositions").transform;
        InitRoleDataDict();
    }

    public PlayerManager(GameFacade facade) : base(facade)
    {  
    }
    /// <summary>
    /// 初始化游戏角色字典
    /// </summary>
    public void InitRoleDataDict()
    {
        roleDataDict.Add(RoleType.Blue, new RoleData(RoleType.Blue, "Prefabs/Hunter_Blue", "Prefabs/Arrow_blue", "Prefabs/Explosion_Blue", rolePositions.Find("BludePlayerPos")));
        roleDataDict.Add(RoleType.Red, new RoleData(RoleType.Red, "Prefabs/Hunter_Red", "Prefabs/Arrow_Red", "Prefabs/Explosion_Red", rolePositions.Find("RedPlayerPos")));
    }
    /// <summary>
    /// 生成游戏角色
    /// </summary>
    public void SpawnRoles()
    {
        foreach ( RoleData roleData in roleDataDict.Values)
        {
            //通过RoleData自身的属性信息来实例化
            GameObject roleGo = GameObject.Instantiate(roleData.RolePrefab, roleData.SpawnPosition, Quaternion.identity);

            roleGo.tag = "Player";//设置tag，用于箭矢的碰撞检测

            if (roleData.RoleType==currentRoleType)//如果当前实例化的游戏角色类型与当前客户端操作的角色类型一致，则保存实例
            {
                currentRoleGo = roleGo;
                currentRoleGo.GetComponent<PlayerInfo>().isLocal = true;
            }
            else//如果是敌对角色
            {
                remoteRoleGo = roleGo;
            }
        }
    }
    /// <summary>
    /// 根据角色类型获取游戏角色的角色信息
    /// </summary>
    /// <param name="roleType"></param>
    /// <returns></returns>
    private RoleData GetRoleData(RoleType roleType)
    {
        RoleData roleData = null;
        roleData = roleDataDict.TryGet(roleType);
        return roleData;
    }
    /// <summary>
    /// 更新本地用户的战绩信息
    /// </summary>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    public void UpdateUserdata(int totalCount, int winCount)
    {
        this.userData.TotalCount = totalCount;
        this.userData.WinCount = winCount;
    }

    /// <summary>
    /// 给游戏角色添加控制脚本
    /// </summary>
    public void AddControllScript()
    {
        currentRoleGo.AddComponent<PlayerMove>();
        PlayerAttack playerAttack =currentRoleGo.AddComponent<PlayerAttack>();
        //获取当前操作角色的角色类型
        RoleType roleType = currentRoleGo.GetComponent<PlayerInfo>().roleType;
        //通过角色类型获取到角色字典中对应的角色信息
        RoleData roleData = GetRoleData(roleType);
        //通过预设的角色信息给角色的箭矢物体赋值
        playerAttack.arrowPrefab = roleData.ArrowPrefab;

        playerAttack.SetPlayerManager(this);
    }
    /// <summary>
    /// 创建同步请求对象，将用于同步操作的脚本都放置到该游戏物体上
    /// </summary>
    public void CreateSyncRequest()
    {
        playerSyncRequest=new GameObject("PlayerSyncRequest");//在场景中创建游戏物体
        MoveRequest moveRequest = playerSyncRequest.AddComponent<MoveRequest>();//添加移动同步请求对象
        moveRequest .SetLocalPlayer(currentRoleGo.transform,currentRoleGo.GetComponent<PlayerMove>());//告知移动同步对象，本地角色的位置信息
        moveRequest.SetRemotePlayer(remoteRoleGo.transform);//设置敌对角色，用于接收到同步信息以后直接控制敌对角色移动
        shootRequest=playerSyncRequest.AddComponent<ShootRequest>();//添加同步箭矢射击的请求对象
        shootRequest.playerManager = this;
        attackRequest= playerSyncRequest.AddComponent<AttackRequest>();//添加造成伤害的请求对象
    }
    /// <summary>
    /// 本地发射箭矢，本地创建箭矢物体同时，需要向服务器发送请求，通知其他客户端
    /// </summary>
    /// <param name="arrowPrefab">要发射的箭矢的预制体</param>
    /// <param name="pos">初始化位置</param>
    /// <param name="rotation">发射时的朝向</param>
    public void Shoot(GameObject arrowPrefab,Vector3 pos,Quaternion rotation)
    {
        //实例化箭的游戏物体，设置位置在人物的左手的位置，方向朝向要发射的目标方向
        GameObject.Instantiate(arrowPrefab, pos, rotation).GetComponent<ArrowMove>().isLocal = true; ;       

        facade.PlayNormalSound(AudioManager.Sound_Timer);
        //发送射击箭矢的请求
        shootRequest.SendRequest(arrowPrefab.GetComponent<ArrowMove>().RoleType,pos,rotation.eulerAngles);
    }

    /// <summary>
    /// 接收到服务器响应的其他客户端的射击事件
    /// </summary>
    /// <param name="arrowPrefab">要发射的箭矢的预制体</param>
    /// <param name="pos">初始化位置</param>
    /// <param name="rotation">发射时的朝向</param>
    public void RemoteShoot(RoleType roleType, Vector3 pos, Vector3 rotation)
    {
        //先通过角色类型获取到角色信息中保存的箭矢的预制体
        GameObject arrowPrefab = GetRoleData(roleType).ArrowPrefab;

        //由于GameObject.Instantiate方法实例化物体不能通过vector3来指定方向，所以先获取到游戏物体的transform组件
        Transform arrowTransform = GameObject.Instantiate(arrowPrefab).transform;
        arrowTransform.position = pos;
        arrowTransform.eulerAngles = rotation;
    }
    /// <summary>
    /// 向服务器发送造成伤害的请求
    /// </summary>
    /// <param name="damage"></param>
    public void SendAttack(int damage)
    {
        attackRequest.SendRequest(damage);
    }
    /// <summary>
    /// 游戏结束，销毁游戏物体
    /// </summary>
    public void GameOver()
    {
        GameObject.Destroy(currentRoleGo);
        GameObject.Destroy(playerSyncRequest);
        GameObject.Destroy(remoteRoleGo);
        shootRequest = null;
        attackRequest = null;
    }
}

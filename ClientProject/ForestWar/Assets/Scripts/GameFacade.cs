
using Common;
using UnityEngine;

/// <summary>
/// 用于统筹管理各个模块的管理者实例，外界通过该类中的接口调用各个管理者，降低耦合性
/// </summary>
public class GameFacade : MonoBehaviour
{
    private static GameFacade _instance; //单例模式的唯一实例
    private bool isEnterPlaying = false;//异步开始游戏标志位
    public static GameFacade Instance //唯一实例的获取方法
    {
        get { return _instance; }
    }

    private UIManager uiManager;

    private PlayerManager playerManager;

    private AudioManager audioManager;

    private CameraManager cameraManager;

    private RequestManager requestManager;

    private ClientManager clientManager;

    /// <summary>
    /// 初始化方法
    /// </summary>
    private void InitManager()
    {
        //实例化各个模块管理者实例
        uiManager = new UIManager(this);
        audioManager = new AudioManager(this);
        playerManager = new PlayerManager(this);
        cameraManager = new CameraManager(this);
        requestManager = new RequestManager(this);
        clientManager = new ClientManager(this);
        //初始化各个管理者实例
        uiManager.OnInit();
        audioManager.OnInit();
        playerManager.OnInit();
        cameraManager.OnInit();
        requestManager.OnInit();
        clientManager.OnInit();
    }

    private void UpdateManager()
    {
        //更新各个管理者实例
        uiManager.Update();
        audioManager.Update();
        playerManager.Update();
        cameraManager.Update();
        requestManager.Update();
        clientManager.Update();
    }

    /// <summary>
    /// 销毁管理者实例
    /// </summary>
    private void DestroyManager()
    {
        //销毁各个管理者实例
        uiManager.OnDestroy();
        audioManager.OnDestroy();
        playerManager.OnDestroy();
        cameraManager.OnDestroy();
        requestManager.OnDestroy();
        clientManager.OnDestroy();
    } 

    void Awake() 
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this; //在unity初始化该脚本时，给唯一实例赋值
        Screen.SetResolution(1280,800,false);//设置分辨率为1280*800，禁止全屏
    }

    // Use this for initialization
    void Start()
    {
        InitManager();//初始化各个Manager
    }

    // Update is called once per frame
    void Update()
    {
        //异步开始游戏
        if (isEnterPlaying)
        {
            EnterPlaying();
            isEnterPlaying = false;
        }
        UpdateManager();
    }

    public void OnDestroy()
    {
        DestroyManager();//销毁Manager
    }
    /// <summary>
    /// 向RequestManager中的字典中添加Request对象
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="request"></param>
    public void AddRequest(ActionCode actionCode, BaseRequest request)
    {
        requestManager.AddRequest(actionCode, request);
    }

    /// <summary>
    /// 根据RequestCode移除RequestManager对象的管理字典中对应的Request实例
    /// </summary>
    /// <param name="actionCode"></param>
    public void RemoveRequest(ActionCode actionCode)
    {
        requestManager.RemoveRequest(actionCode);
    }
    /// <summary>
    /// 通过ClientManager对象发送请求
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        clientManager.SendRequest(requestCode,actionCode,data);
    }
    /// <summary>
    /// 通过RequestManager处理从服务器端返回的响应结果
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void HandleResponse(ActionCode actionCode, string data)
    {
        requestManager.HandleResponse(actionCode,data);
    }
    /// <summary>
    /// 显示提示信息
    /// </summary>
    /// <param name="msg"></param>
    public void ShowMessage(string msg)
    {
        uiManager.ShowMessage(msg);
    }
    /// <summary>
    /// 循环播放指定的背景音乐
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayBgSound(string soundName)
    {
        audioManager.PlayBgSound(soundName);
    }
    /// <summary>
    /// 播放指定的一般音效
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayNormalSound(string soundName)
    {

       audioManager.PlayNormalSound(soundName);
    }
    /// <summary>
    /// 向PlayerManager实例中添加UserData数据
    /// </summary>
    /// <param name="userData"></param>
    public void SetUserData(UserData userData)
    {
        playerManager.UserData = userData;
    }
    /// <summary>
    /// 获取PlayerManager中的存储的UserData数据
    /// </summary>
    /// <returns></returns>
    public UserData GetUserData()
    {
        return playerManager.UserData;
    }
    /// <summary>
    /// 设置当前角色类型
    /// </summary>
    /// <param name="roleType"></param>
    public void SetCurrentRoleType(RoleType roleType)
    {
        playerManager.SetCurrentRoleType(roleType);
    }

    /// <summary>
    /// 获取当前客户端要操作的游戏角色物体
    /// </summary>
    /// <returns></returns>
    public GameObject GetCurrentRoleGo()
    {
        return playerManager.GetCurrentRoleGo();
    }
    /// <summary>
    /// 异步开始游戏
    /// </summary>
    public void EnterPlayingSync()
    {
        isEnterPlaying = true;
    }
    /// <summary>
    /// 进入游戏
    /// </summary>
    public void EnterPlaying()
    {
        //创建双方的游戏角色
        playerManager.SpawnRoles();
        //摄像机开启跟随当前操作的游戏物体
        cameraManager.FollowTarget();
    }

    /// <summary>
    /// 计时结束以后，开始控制角色
    /// </summary>
    public void StartPlaying()
    {
        playerManager.AddControllScript();//给角色添加控制脚本
        playerManager.CreateSyncRequest();//创建数据同步对象
    }
    /// <summary>
    /// 发送造成伤害的请求
    /// </summary>
    public void SendAttack(int damage)
    {
        playerManager.SendAttack(damage);
    }
    /// <summary>
    /// 游戏结束，销毁游戏物体，摄像机开始播放漫游动画
    /// </summary>
    public void GameOver()
    {
        //摄像机再次开始漫游
        cameraManager.WalkThroughScene();
        playerManager.GameOver();
    }
    /// <summary>
    /// 更新本地保存的用户战绩信息
    /// </summary>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    public void UpdateUserdata(int totalCount, int winCount)
    {
       playerManager.UpdateUserdata(totalCount,winCount);
    }
}

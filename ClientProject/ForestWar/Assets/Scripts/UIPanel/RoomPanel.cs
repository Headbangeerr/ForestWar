using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    private Text localPlayerUsername;
    private Text localPlayerTotalCount;
    private Text localPlayerWinCount;

    private Text enemyPlayerUsername;
    private Text enemyPlayerTotalCount;
    private Text enemyPlayerWinCount;

    private Button startButton;
    private Button quitButton;

    private Transform bluePanel;
    private Transform redPanel;

    //两个面板的初始位置
    private Vector2 bluePanelPos;
    private Vector2 redPanelPos;
   
    //用于异步显示房间内的玩家信息
    private UserData userData = null;
    private UserData ud1 = null;
    private UserData ud2 = null;
    //用于异步出栈面板
    private bool isPopPanel = false;  
    //退出房间请求对象
    private QuitRoomRequest quitRoomRequest;
    //开始游戏请求对象
    private StartGameRquest startGameRquest;
    void Awake()
    {
        //初始化UI组件
        bluePanel = transform.Find("BluePanel");
        redPanel = transform.Find("RedPanel"); 

        localPlayerUsername = transform.Find("BluePanel/Username").GetComponent<Text>();
        localPlayerTotalCount = transform.Find("BluePanel/TotalCount").GetComponent<Text>();
        localPlayerWinCount = transform.Find("BluePanel/WinCount").GetComponent<Text>();

        enemyPlayerUsername = transform.Find("RedPanel/Username").GetComponent<Text>();
        enemyPlayerTotalCount = transform.Find("RedPanel/TotalCount").GetComponent<Text>();
        enemyPlayerWinCount = transform.Find("RedPanel/WinCount").GetComponent<Text>();

        startButton = transform.Find("StartButton").GetComponent<Button>();
        quitButton = transform.Find("QuitButton").GetComponent<Button>();

        //添加摁钮组件的监听事件
        startButton.onClick.AddListener(OnStartButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);

        //获取两个面板的原始位置
        bluePanelPos = bluePanel.localPosition;
        redPanelPos = redPanel.localPosition;
        //退出房间请求对象
        quitRoomRequest = GetComponent<QuitRoomRequest>();
        //开始游戏请求对象
        startGameRquest = GetComponent<StartGameRquest>();

        //EnterAnim();
    }

    void Update()
    {
        //当异步方法修改变量的值以后，触发非异步方法
        if (userData!=null)
        {
            //显示房主信息
            SetBluePlayerInfo(userData.Username, userData.TotalCount.ToString(), userData.WinCount.ToString());
            //清空另一名玩家的信息
            ClearRedPlayerInfo();
            userData = null;
        }
        if (ud1!=null)//异步更新房间玩家信息
        {
            //如果房主不为空，先显示房主信息
            SetBluePlayerInfo(ud1.Username, ud1.TotalCount.ToString(), ud1.WinCount.ToString());
            if (ud2!=null)//如果红方玩家的信息不为空，代表是刚刚进入房间，显示新加入的玩家信息
            {
                SetRedPlayerInfo(ud2.Username, ud2.TotalCount.ToString(), ud2.WinCount.ToString());
            }//如果红方玩家为空，则代表是刚刚退出房间，需要清空该名玩家的信息
            else
            {
                ClearRedPlayerInfo();
            }
            ud1 = null;
            ud2 = null;
        }        
        if (isPopPanel)//异步出栈当前面板
        {
            uiManager.PopPanel(this);
            isPopPanel = false;
        }       
    }
    /// <summary>
    /// 面板进入触发的方法
    /// </summary>
    public override void OnEnter()
    {
        //if (bluePanel!=null)
        //{
        //    EnterAnim();
        //}

        /* EnterAnim方法必须在获取createRoomRequest对象之前，否则createRoomRequest会无法初始化，因为在EnterAnim方法
         * 中的setActiv方法之前，ui物体一直处于无可用的状态，所以ui物体上的createRoomRequest脚本一直没有初始化，也就
         * 取不到实例对象，只有在EnterAnim以后才会启用物体，才能成功获取到createRoomRequest实例
         * 
         */
        EnterAnim();
        //if (createRoomRequest == null)
        //{
        //    createRoomRequest = GetComponent<CreateRoomRequest>();
        //}               
    }

    public override void OnPause()
    {
        ExitAnim();
    }

    public override void OnResume()
    {
        EnterAnim();
    }
    public override void OnExit()
    {
        ExitAnim();
    }

    /// <summary>
    /// 设置房主玩家的信息
    /// </summary>
    /// <param name="username"></param>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    public void SetBluePlayerInfo(string username, string totalCount, string winCount)
    {
        localPlayerUsername.text = username;
        localPlayerTotalCount.text = "总场数：" + totalCount;
        localPlayerWinCount.text = "胜场：" + winCount;
    }
    /// <summary>
    /// 异步的形式显示房间信息
    /// </summary>
    /// <param name="ud1">房主信息</param>
    /// <param name="ud2">除房主外的另一名玩家信息</param>
    public void SetRoomDataSync(UserData ud1, UserData ud2)
    {
        this.ud1 = ud1;
        this.ud2 = ud2;
    }
    /// <summary>
    /// 异步方式修改房间面板中的房主信息
    /// </summary>
    public void SetBluePlayerInfoSync()
    {
        //获取以登录的房主的信息，触发Update中的非异步方法
        userData = facade.GetUserData();        
    }

    /// <summary>
   /// 清空敌对玩家信息
   /// </summary>
    public void ClearRedPlayerInfo()
    {
        
        enemyPlayerUsername.text = "";
        enemyPlayerTotalCount.text = "等待玩家加入……";
        enemyPlayerWinCount.text = "";
    }

    /// <summary>
    /// 设置敌对玩家的信息
    /// </summary>
    /// <param name="username"></param>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    private void SetRedPlayerInfo(string username, string totalCount, string winCount)
    {
        enemyPlayerUsername.text = username;
        enemyPlayerTotalCount.text = "总场数：" + totalCount;
        enemyPlayerWinCount.text = "胜场：" + winCount;
    }
    /// <summary>
    /// 开始摁钮监听事件
    /// </summary>
    private void OnStartButtonClick()
    {
        startGameRquest.SendRequest();             
    }

    public void OnStartGameResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Fail)
        {
            uiManager.ShowMessageSync("您不是房主，请等待房主开始游戏！");
        }
        else if (returnCode==ReturnCode.Success)//如果返回值为success，表示开始游戏
        {            
            uiManager.PushPanelSync(UIPanelType.Game);//将游戏面板入栈 
            //进入游戏 
            //facade.EnterPlaying();//涉及到游戏物体的实例创建，不能在非主线程调用   
            facade.EnterPlayingSync();
        }
        
    }

    /// <summary>
    /// 退出摁钮监听事件
    /// </summary>
    private void OnQuitButtonClick()
    {
        //发送退出房间请求
        quitRoomRequest.SendRequest();
    }
    /// <summary>
    /// 当接收到服务器端对退出房间请求的响应以后的处理
    /// </summary>
    public void OnQuitRoomResponse()
    {
        //uiManager.PopPanel();//非主线程调用ui物体，抛出异常
        isPopPanel = true;
    }
    /// <summary>
    /// 面板进入动画
    /// </summary>
    private void EnterAnim()
    {        
        //启用ui物体
        gameObject.SetActive(true);
        //左边的蓝色面板从屏幕左侧移入
        bluePanel.transform.position=new Vector2(-Screen.width,bluePanelPos.y);
        bluePanel.transform.DOLocalMove(bluePanelPos,.4f);
        //右边的红色面板从屏幕右侧移入
        redPanel.transform.position=new Vector2(Screen.width,redPanelPos.y);
        redPanel.transform.DOLocalMove(redPanelPos,.4f);
        //摁钮动画
        startButton.transform.localScale=Vector3.zero;
        startButton.transform.DOScale(1,.4f);

        quitButton.transform.localScale = Vector3.zero;
        quitButton.transform.DOScale(1,.4f);

    }
    /// <summary>
    /// 面板退出动画
    /// </summary>
    private void ExitAnim()
    {
        //面板各个向屏幕两侧移出
        bluePanel.DOLocalMove(new Vector2(-Screen.width,bluePanelPos.y),.4f );
        redPanel.DOLocalMove(new Vector2(Screen.width,redPanelPos.y),.4f );
        //摁钮原地缩小消失
        startButton.transform.DOScale(0,.4f);
        quitButton.transform.DOScale(0,.4f).OnComplete(()=>gameObject.SetActive(false));//关闭面板后，禁用ui物体
    }	
}

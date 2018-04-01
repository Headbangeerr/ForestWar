using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    private RectTransform userInfo;
    private RectTransform roomList;
    private Vector2 userInfoPos;
    private Vector2 roomListPos;
    private VerticalLayoutGroup roomLayout;
    private GameObject roomItemPrefab;

    private ListRoomRequest listRoomRequest;
    private CreateRoomRequest createRoomRequest;
    private JoinRoomRequest joinRoomRequest;
    private List<UserData> userDataList = null;//用于异步显示房间列表
    //同步全局变量，实现在非主线程中显示ui面板
    private UserData ud1 = null;
    private UserData ud2 = null;

   
    void Awake()
    {
        //获取面板中的活动组件
        userInfo = transform.Find("UserInfo").GetComponent<RectTransform>();
        roomList = transform.Find("RoomList").GetComponent<RectTransform>();
        //房间列表刷新摁钮点击事件
        transform.Find("RoomList/ReFreshButton").GetComponent<Button>().onClick.AddListener(OnRefreshButtonClick);
        //添加关闭摁钮的点击触发事件
        transform.Find("RoomList/CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseButtonClick);
        //创建房间摁钮触发事件
        transform.Find("RoomList/CreateRoomButton").GetComponent<Button>().onClick.AddListener(OnCreateRoomButtonClick);
        //获取两个面板在中间的原始位置
        userInfoPos = userInfo.localPosition;
        roomListPos = roomList.localPosition;
        //获取RoomItem预制体
        roomItemPrefab=Resources.Load<GameObject>("UIPanel/RoomItem") as GameObject;      
        roomLayout = transform.Find("RoomList/ScrollRect/Layout").GetComponent<VerticalLayoutGroup>();
        //获取房间列表请求对象
        listRoomRequest = GetComponent<ListRoomRequest>();
        //获取创建房间请求脚本
        createRoomRequest = GetComponent<CreateRoomRequest>();
        //获取加入房间请求对象
        joinRoomRequest = GetComponent<JoinRoomRequest>();
    }

    void Update()
    {
        //异步形式显示房间列表
        if (userDataList!=null)
        {
            LoadRoomList(userDataList);
            userDataList = null;
        }
        if (ud1!=null||ud2!=null)
        {
            RoomPanel roomPanel = uiManager.PushPanel(UIPanelType.Room) as RoomPanel;//通过返回值获取roomPanel对象
            roomPanel.SetRoomDataSync(ud1,ud2);//异步显示房间内玩家信息
            ud1 = null;
            ud2 = null;
        }
    }
    /// <summary>
    /// 入栈时被调用的触发方法
    /// </summary>
    public override void OnEnter()
    {      
        EnterAnim();
        //显示用户信息
        SetUserInfo();
        //在该面板每次显示时都需要重新获取房间列表
        listRoomRequest.SendRequest();       
    }
    /// <summary>
    /// 页面被覆盖时触发方法
    /// </summary>
    public override void OnPause()
    {
        ExitAnim();       
    }

    /// <summary>
    /// 从暂停休眠状态恢复启用
    /// </summary>
    public override void OnResume()
    {
        Debug.Log("RoomList重新被激活了");
        //在该面板每次显示时都需要重新获取房间列表       
        EnterAnim();
        listRoomRequest.SendRequest();
    }
    /// <summary>
    /// 创建房间摁钮触发函数
    /// </summary>
    private void OnCreateRoomButtonClick()
    {        
        //利用入栈操作的返回值，将RoomPanel注入到createRoomRequest对象中，用于控制RoomPanel的ui
        createRoomRequest.SetRoomPanel(uiManager.PushPanel(UIPanelType.Room));
        createRoomRequest.SendRequest();//发送创建房间的请求        
    }
    /// <summary>
    /// 刷新房间列表摁钮触发函数
    /// </summary>
    private void OnRefreshButtonClick()
    {
        //重新发送获取房间列表的请求
        listRoomRequest.SendRequest();
    }
    /// <summary>
    /// 关闭摁钮触发函数
    /// </summary>
    private void OnCloseButtonClick()
    {
        PlayClickSound();
        uiManager.PopPanel(this);
    }
    /// <summary>
    /// 被出栈是触发的函数
    /// </summary>
    public override void OnExit()
    {
        ExitAnim();
    }
    /// <summary>
    /// 显示用户信息面板中的各个信息
    /// </summary>
    private void SetUserInfo()
    {
        UserData userData = facade.GetUserData();
        transform.Find("UserInfo/Username").GetComponent<Text>().text = userData.Username;
        transform.Find("UserInfo/TotalCount").GetComponent<Text>().text = "总场数："+userData.TotalCount;
        transform.Find("UserInfo/WinCount").GetComponent<Text>().text = "胜场："+userData.WinCount;        
    }
    /// <summary>
    /// 异步方式加载房间列表
    /// </summary>
    public void LoadRoomListSync(List<UserData> userDataList)
    {
        this.userDataList = userDataList;//通过全局变量形式实现异步加载房间列表
    }
    /// <summary>
    /// 加载房间列表
    /// </summary>
    private  void LoadRoomList(List<UserData> userDataList)
    {
        //在加载房间列表之前先清空列表，防止重复加载
        RoomItem[] roomItems = roomLayout.GetComponentsInChildren<RoomItem>();
        //逐个调用销毁自身的方法
        foreach (RoomItem roomItem in roomItems)
        {
            roomItem.DestorySelf();
        }

        foreach (UserData userData in userDataList) //逐个创建房间信息对象，并添加到房间列表布局中
        {
            GameObject roomItem = GameObject.Instantiate(roomItemPrefab);
            roomItem.transform.SetParent(roomLayout.transform);
            //逐个设置房间信息
            roomItem.GetComponent<RoomItem>().SetRoomItemInfo(userData.Id,userData.Username,userData.TotalCount,userData.WinCount,this);
        }
              
        //获取所有子物体中RoomItem对象的个数，即房间个数
        int roomCount = GetComponentsInChildren<RoomItem>().Length;
        //房间列表原来的大小
        Vector2 size = roomLayout.GetComponent<RectTransform>().sizeDelta;
        //单个房间信息框的高度
        float roomItemHeight = roomItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
        //重新设置房间列表的长度，新的长度等于所有房间信息的个数×（一个roomItem的高度+两个roomItem之间的间隙）
        roomLayout.GetComponent<RectTransform>().sizeDelta=new Vector2(size.x,roomCount*(roomItemHeight+roomLayout.spacing));
    }
    /// <summary>
    /// 加入指定房间，被RoomItem进行调用
    /// </summary>
    /// <param name="id">房间id</param>
    public void OnJoinButtonClick(int id)
    {
        joinRoomRequest.SendRequest(id);
    }
    /// <summary>
    /// 当加入房间的请求收到响应时的处理
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="ud1">房主信息</param>
    /// <param name="ud2">除了房主的另一名玩家的信息</param>
    public void OnJoinResponse(ReturnCode returnCode,UserData ud1,UserData ud2)
    {
        switch (returnCode)
        {
            case ReturnCode.NotFound:
                uiManager.ShowMessageSync("房间已销毁，无法加入！");
                break;
            case ReturnCode.Fail:
                uiManager.ShowMessageSync("房间已满！");
                break;
            case ReturnCode.Success://成功加入房间，显示房间面板
                //RoomPanel roomPanel = uiManager.PushPanel(UIPanelType.Room) as RoomPanel;//通过返回值获取roomPanel对象
                //roomPanel.SetRoomDataSync(ud1,ud2);//异步显示房间内玩家信息
                /*
                 * 上述代码由于调用了uiManager.PushPanel，该方法实例化了游戏物体，所以只能在主线程中调用，这里会报错。
                 * 这里只能使用同步方式进行显示
                 */
                this.ud1 = ud1;
                this.ud2 = ud2;
                break;
        }
    }
    /// <summary>
    /// 接收到更新战绩信息的响应
    /// </summary>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    public void OnUpdateResultResponse(int totalCount, int winCount)
    {
        facade.UpdateUserdata(totalCount, winCount);
        SetUserInfo();
    }
    /// <summary>
    /// 进入动画
    /// </summary>
    private void EnterAnim()
    {
        gameObject.SetActive(true);//激活面板
        //左边的面板设置在屏幕左侧                       
        userInfo.localPosition =new Vector2(-Screen.width,0);
        userInfo.transform.DOLocalMove(userInfoPos, .5f);
        //右边的面板设置在屏幕右侧        
        roomList.localPosition = new Vector2(Screen.width, 0);
        roomList.transform.DOLocalMove(roomListPos,.5f);
    }
    /// <summary>
    /// 退出动画
    /// </summary>
    private void ExitAnim()
    {
        userInfo.DOLocalMove(new Vector2(-Screen.width, 0), .4f);
        //播放退出动画，并禁用自身，节省资源
        roomList.DOLocalMove(new Vector2(Screen.width,0), .4f).OnComplete(()=>gameObject.SetActive(false));        
    }

}

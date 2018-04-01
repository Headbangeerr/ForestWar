using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private Button closeButton;//面板的关闭摁钮  
    private InputField usernameField;//用户名输入框
    private InputField passwordField;//密码输入框

    private LoginRequest loginRequest;

    void Awake()
    {
        loginRequest = GetComponent<LoginRequest>();//获取自身上的LoginRequest脚本
        closeButton = transform.Find("CloseButton").GetComponent<Button>();//获取关闭摁钮    
        usernameField = transform.Find("Username/UsernameInput").GetComponent<InputField>();//获取输入域组件
        passwordField = transform.Find("Password/PasswordInput").GetComponent<InputField>();
    }
    //private Button loginButton;
    //private Button registerButton;
    /// <summary>
    /// 面板入栈动画
    /// </summary>
    private void EnterAnim()
    {
        gameObject.SetActive(true);//物体启用
        transform.localScale = Vector2.zero;//初始化时大小为零，即不显示
        transform.localPosition = new Vector2(Screen.width, transform.localPosition.y);
        transform.DOScale(1, .4f);//实现从小到大的动画效果
        transform.DOLocalMove(Vector3.zero, .4f);
    }
    /// <summary>
    /// 面板出栈动画
    /// </summary>
    private void ExitAnim()
    {
        transform.DOScale(0, .4f);//实现从小到大的动画效果
        transform.DOLocalMove(new Vector2(Screen.width, 0), .4f).OnComplete(() => gameObject.SetActive(false));//将面板移到屏幕边缘，并禁用自身
    }
    /// <summary>
    /// 面板初始化方法
    /// </summary>
    public override void OnEnter()
    {        
        base.OnEnter();                
        //closeButton.onClick.AddListener(OnCloseClick);//添加点击事件触发的函数
        /*
         *上面这种动态绑定添加摁钮事件的形式存在一些问题，由于OnEnter方法会重复调用，使用代码实现的动态绑定也会出现重复，
         * 最好应该是在Awake方法中实现最初的动态绑定，这里是使用的引擎编辑器拖拽绑定的形式
         */       
        EnterAnim();
    }
    /// <summary>
    /// 被覆盖以后的触发函数
    /// </summary>
    public override void OnPause()
    {
        ExitAnim();
    }

    public override void OnResume()
    {
        EnterAnim();
    }
    /// <summary>
    /// 注册摁钮点击事件
    /// </summary>
    public void OnRegisterButtonClick()
    {
        PlayClickSound();
        uiManager.PushPanel(UIPanelType.Register);//显示注册面板
    }
    /// <summary>
    /// 登陆摁钮点击事件
    /// </summary> 
    public void OnLoginButtonClick()
    {
        PlayClickSound();
        string msg = "";
        //对输入框内容进行校验
        if (string.IsNullOrEmpty(usernameField.text))
        {
            msg += "用户名不能为空！";
        }
        if (string.IsNullOrEmpty(passwordField.text))
        {
            msg += "密码不能为空！";
        }
        if (msg != "")//如果验证失败，则跳窗显示提示信息
        {
            Debug.Log(msg);
            uiManager.ShowMessage(msg);
            return;            
        }
        //通过验证后，利用Request对象发送登录请求
        loginRequest.SendRequest(usernameField.text,passwordField.text);
    }
    /// <summary>
    /// 关闭摁钮点击事件
    /// </summary>
    public void OnCloseButtonClick()
    {        
        PlayClickSound();
        uiManager.PopPanel(this);
        //transform.DOScale(0, .4f);//实现从小到大的动画效果
        //transform.DOLocalMove(new Vector2(Screen.width,0), .4f).OnComplete(()=>uiManager.PopPanel());//将面板移到屏幕边缘，动画结束后将面板出栈
    }
    /// <summary>
    /// 处理登录响应 
    /// </summary>
    /// <param name="returnCode"></param>
    public void OnLoginResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {            
            //uiManager.ShowMessageSync("登陆成功！");
            uiManager.PushPanelSync(UIPanelType.RoomList);//异步方式将房间面板入栈
            //uiManager.PushPanel(UIPanelType.RoomList);//显示房间面板
            /*
             * 由于这里不在主线程，所以无法通过PushPanel方法加载ui面板，这里同样需要一个异步方法来实现在非主线程
             * 中对ui元素的操控
             */
        }
        else
        {
            /*
             * 由于接收响应的过程是通过异步的形式触发的，所以该函数也不是在主线程中调用的，
             * 所以这里想要通过uiManager的ShowMessage方法来修改Text组件的值是不允许的，使用下述代码
             * 会抛出异常“set_enabled can only be called from the main thread.”，提示对ui的操作需要在
             * 主线程中进行。
             */
            //uiManager.ShowMessage("用户名或密码错误，请重新输入！");
            uiManager.ShowMessageSync("用户名或密码错误，请重新输入！");//通过异步方式显示提示信息
        }
    }
    /// <summary>
    /// 在面板出栈时的处理
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        ExitAnim();//播放关闭动画
    }
}

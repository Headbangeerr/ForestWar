using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    private InputField usernameField;
    private InputField passworField;
    private InputField rePwdField;
    private Button closeButton;
    private Button registerButton;

    private RegisterRequest registerRequest;
    private void Start()
    {
        //获取相应的输入框组件
        usernameField = transform.Find("Username/UsernameInput").GetComponent<InputField>();
        passworField = transform.Find("Password/PasswordInput").GetComponent<InputField>();
        rePwdField = transform.Find("RepeatPassword/PasswordInput").GetComponent<InputField>();
        closeButton = transform.Find("CloseButton").GetComponent<Button>();
        registerButton = transform.Find("RegisterButton").GetComponent<Button>();
        //绑定摁钮触发事件
        closeButton.onClick.AddListener(OnCloseButtonClick);
        registerButton.onClick.AddListener(OnRegisterButtonClick);
        //获取自身的绑定脚本的Register组件
        registerRequest = transform.GetComponent<RegisterRequest>();
    }
    /// <summary>
    /// 
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        gameObject.SetActive(true);//启用组件
        transform.localScale = Vector2.zero;//初始化时大小为零，即不显示
        transform.localPosition = new Vector2(Screen.width, transform.localPosition.y);
        transform.DOScale(1, .4f);//实现从小到大的动画效果
        transform.DOLocalMove(Vector3.zero, .4f);
    }
    /// <summary>
    /// 注册摁钮点击事件
    /// </summary>
    private void OnRegisterButtonClick()
    {
        PlayClickSound();
        string msg = "";
        if (string.IsNullOrEmpty(usernameField.text))
        {
            msg += "用户名不能为空！";
        }
        if (string.IsNullOrEmpty(passworField.text))
        {
            msg += "\n密码不能为空！";
        }
        if (passworField.text!=rePwdField.text)
        {
            msg += "\n两次密码不一致";
        }
        if (msg!="")
        {
            uiManager.ShowMessage(msg);return;           
        }
        //发送注册请求
        registerRequest.SendRequest(usernameField.text,passworField.text);
       
    }
    /// <summary>
    /// 关闭摁钮触发事件
    /// </summary>
    private void OnCloseButtonClick()
    {
        PlayClickSound();
        uiManager.PopPanel(this);
    }

    public void OnRegisterResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            uiManager.ShowMessageSync("注册成功！");
        }
        else
        {
            uiManager.ShowMessageSync("用户名重复！");
        }
    }
    /// <summary>
    /// 在ui框架中调用pop方法出栈面板时，会自动触发OnExit方法
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        transform.DOScale(0, .4f);//实现从小到大的动画效果
        //将面板移到屏幕边缘，动画结束后将面板出栈
        transform.DOLocalMove(new Vector2(Screen.width, 0), .4f).OnComplete(() => gameObject.SetActive(false));
    }
}

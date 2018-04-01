using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    private Button loginButton;
    private Animator loginButtonAnimator;//登陆摁钮动画组件
    /// <summary>
    /// 面板入栈显示时要处理的操作
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();

        loginButton= transform.Find("LoginButton").GetComponent<Button>();//获取子物体登录摁钮  
        loginButtonAnimator = loginButton.transform.GetComponent<Animator>();//获取登陆摁钮的动画组件
        loginButton.enabled = true;
        //loginButton.onClick.AddListener(OnLoginButtonClick);       
    }
    /// <summary>
    /// 面板被其他面板覆盖时要处理的操作
    /// </summary>
    public override void OnPause()
    {
        base.OnPause();
        Debug.Log("pause");
        loginButtonAnimator.enabled = false;//先将摁钮的动画组件禁用，否则无法正常显示DOTween动画
        //播放缩小消失动画结束后，将登陆摁钮禁用
        loginButton.transform.DOScale(0, .5f).OnComplete(()=>loginButton.gameObject.SetActive(false));
    }
    /// <summary>
    /// 重新被激活以后要处理的操作
    /// </summary>
    public override void OnResume()
    {
        base.OnResume();        
        loginButton.gameObject.SetActive(true);//回复登陆摁钮显示
        loginButton.transform.DOScale(1, .3f).OnComplete(()=> loginButtonAnimator.enabled = true);
        /*
         * 上述代码出现的问题：当通过DOTween插件播放完摁钮出现的动画以后，再启用Animator组件，显示摁钮高亮动画的时候，摁钮消失，不再显示
         * 这是因为Button的Animator组件分为4种状态，当重新开启动画组件时，默认是处在Normal状态，这是需要在Normal状态下将摁钮的scale调整为1
         */
    }

    /// <summary>
    /// 登录摁钮点击事件
    /// </summary>
    public void OnLoginButtonClick()
    {
        PlayClickSound();

        uiManager.PushPanel(UIPanelType.Login);//显示登录面板
    }
}

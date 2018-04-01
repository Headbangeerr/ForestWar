using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MessagePanel : BasePanel
{
    private CanvasGroup canvasGroup;
    private Text text;
    private float showTime=1;//提示框的停留时间
    private string message = null;//用于异步显示提示信息的中转字符串

    void Update()
    {
        /*
         * 通过Update方法实现异步显示提示信息
         */
        if (message!=null)
        {
            Debug.Log("异步显示消息！");
            ShowMessage(message);
            message = null;
        }
    }
    /// <summary>
    /// 面板被创建时执行的初始化操作
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        canvasGroup = GetComponent<CanvasGroup>();//获取自身的CanvasGroup组件用于实现渐隐效果
        uiManager.InjectMessagePanel(this);//通过父类中的UIManager对象，将自身实例注入，方便其他面板使用自身实例
        text = transform.Find("MessageText").GetComponent<Text>();//获取子物体中的Text组件
        canvasGroup.alpha = 0;
        text.enabled = false;//禁用组件，在初始化后不显示，只有其他模块通过接口ShowMessage才显示信息
    }
    /// <summary>
    /// 使用异步方式显示提示信息
    /// </summary>
    /// <param name="msg"></param>
    public void ShowMessageSync(string msg)
    {
        message = msg;
    }
    /// <summary>
    /// 显示提示信息
    /// </summary>
    /// <param name="msg">要显示的提示信息</param>
    public void ShowMessage(string msg)
    {
        //通过该方法可以将ui面板调整自身hierarchy面板上的顺序到最后一个，即显示在所有ui物体之上
        text.enabled = true;
        transform.SetAsLastSibling();
        text.text = msg;       
        canvasGroup.DOFade(1,.5f);//透明度渐变为1，并显示             
        Invoke("HideMessage",showTime);//延时调用隐藏方法
    }
    /// <summary>
    /// 隐藏提示框
    /// </summary>
    private void HideMessage()
    {
        canvasGroup.DOFade(0, .5f);//使用DOTween插件实现渐隐
        //text.material.DOFade(0, .5f);//使用DOTween实现ui渐隐，动画持续时间为0.5s
    }
}

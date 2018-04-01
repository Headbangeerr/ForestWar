using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各种类型面板的基类，用于存放各个面板的共有方法
/// </summary>
public class BasePanel : MonoBehaviour
{
    protected UIManager uiManager;//获取UIManager对象，用于各个面板调用UIManager的接口实现相关业务功能
    protected GameFacade facade;//UIManager对象在实例各个面板过程中，会将自身的保存的GameFacade实例注入到各个面板中
    public UIManager UIManager
    {
        set
        {
            this.uiManager = value;            
        }
    }

    public GameFacade Facade
    {
        set { facade = value; }
    }
    /// <summary>
    /// 通过GameFacade播放摁钮点击的音效
    /// </summary>
    protected void PlayClickSound()
    {
        facade.PlayNormalSound(AudioManager.Sound_ButtonClick);
    }

    /// <summary>
    /// 界面开启
    /// </summary>
    public virtual void OnEnter()
    {
        
    }
    /// <summary>
    /// 界面暂停
    /// </summary>
    public virtual void OnPause()
    {

    }
    /// <summary>
    /// 界面重新被激活
    /// </summary>
    public virtual void OnResume()
    {

    }
    /// <summary>
    /// 界面被关闭
    /// </summary>
    public virtual void OnExit()
    {

    }
}

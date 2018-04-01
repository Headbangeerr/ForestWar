using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 所有管理者控制类的基类
/// </summary>
public class BaseManager
{
    protected GameFacade facade;//提供给子类的接口管理类对象，用于不同模块的管理者对象通过该对象来访问其他模块的管理者

    public BaseManager(GameFacade facade)
    {
        this.facade = facade;
    }
    /// <summary>
    /// 初始化Manager
    /// </summary>
    public virtual void OnInit()
    {        
    }

    public virtual void Update()
    {
        
    }
    /// <summary>
    /// 销毁Manager
    /// </summary>
    public virtual void OnDestroy()
    {
    }

}

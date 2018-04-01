using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
/// <summary>
/// 各种类型请求的基类
/// </summary>
public class BaseRequest : MonoBehaviour
{

    protected RequestCode requestCode=RequestCode.None;//自身所对应的RequestCode
    protected ActionCode actionCode=ActionCode.None;

    protected GameFacade gameFacade;//用于访问其他模块的外部接口   
	// Use this for initialization
	public virtual void Awake () {
       gameFacade=GameFacade.Instance;	 
	    //所有类型的Request对象在被引擎初始化时，将自身的实例添加至RequestManager对象的字典中
        Debug.Log("addRequest:"+actionCode);
       gameFacade.AddRequest(actionCode,this);
	}
    /// <summary>
    /// 子类在发送请求时也都需要发送自身的requestCode与actionCode，放在父类里可以减少子类的操作
    /// </summary>
    /// <param name="data"></param>
    protected void SendRequest(string data)
    {       
        gameFacade.SendRequest(requestCode,actionCode,data);//通过运行时多态，各种子类都可以使用同一个方法
    }
    /// <summary>
    /// 发送请求
    /// </summary>
    public virtual void SendRequest()
    {
    }
    /// <summary>
    /// RequestManager接收来自服务期端的响应
    /// </summary>
    /// <param name="data">从服务器端返回的数据</param>
    public  virtual void OnResponse(string data) { }

    /// <summary>
    /// 销毁Request对象时要做的处理
    /// </summary>
    public virtual void OnDestroy()
    {
        gameFacade.RemoveRequest(actionCode);
    }
}

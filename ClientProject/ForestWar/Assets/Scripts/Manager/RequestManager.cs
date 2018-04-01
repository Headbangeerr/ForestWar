using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
/// <summary>
/// 用于管理所有Request对象
/// </summary>
public class RequestManager : BaseManager {
    //用于保存所有类型的Request对象，在客户端，每一种methodCode对应一种Request
    private Dictionary<ActionCode, BaseRequest> requestDic = new Dictionary<ActionCode, BaseRequest>();
    public RequestManager(GameFacade facade) : base(facade)
    {
    }
    /// <summary>
    /// 由于Request类都是继承了MonoBehavior，所以需要挂载到游戏物体上，所以在本控制类中不能通过new的形式来创建，需要交由Unity在各个脚本awake时
    /// 进行实例化，因此需要在这里提供一个添加方法，用于各个脚本在awake的时候将自己的实例添加到RequestManager对象的词典中
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="request"></param>
    public void AddRequest(ActionCode actionCode,BaseRequest request)
    {
        requestDic.Add(actionCode,request);
    }
    /// <summary>
    /// 根据ActionCode移除管理字典中对应的Request实例
    /// </summary>
    /// <param name="actionCode"></param>
    public void RemoveRequest(ActionCode actionCode)
    {
        requestDic.Remove(actionCode);
    }
    /// <summary>
    /// 处理服务期端的响应
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void HandleResponse(ActionCode actionCode, string data)
    {
        BaseRequest request = requestDic.TryGet(actionCode);//获取到发送请求的Request对象
        if (request == null)
        {
            Debug.LogWarning("没有找到ActionCode[" + actionCode+"]对应的Request对象！");
            return;
        }
        request.OnResponse(data);//通过指定的Request对象来处理响应数据
    }
}

using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class LoginRequest : BaseRequest
{
    private LoginPanel loginPanel;

    public override void Awake()
    {
        loginPanel = GetComponent<LoginPanel>();
        this.requestCode = RequestCode.User;//设置自身的RequestCode与ActionCode
        this.actionCode = ActionCode.Login;
        /*上述初始化内容一定要放在父类的Awake方法之前，
         * 因为父类的Awake方法中需要根据actionCode将自身实例存放至管理字典中,
         * 如果放在父类Awake方法之后，默认是按照父类中的默认类型None来保存的
         */
        base.Awake();
    }
    /// <summary>
    /// 发送登陆验证请求
    /// </summary>
    /// <param name="username">ui传入的用户名</param>
    /// <param name="password">ui传入的密码</param>
    public void SendRequest(string username, string password)
    {
        //通过LitJson插件合成json字符串
        JsonData jsonData=new JsonData();
        jsonData["username"] = username;
        jsonData["password"] = password;

        //string data = username + "," + password;//拼接要传送的数据    
        base.SendRequest(JsonMapper.ToJson(jsonData));
    }
    /// <summary>
    /// 服务器端返回响应
    /// </summary>
    /// <param name="data">服务器端返回的响应数据</param>
    public override void OnResponse(string data)
    {
        //ReturnCode returnCode = (ReturnCode) int.Parse(data);//先将int类型转换为对应的枚举类型

        //解析Json字符串
        JsonData jsonData = JsonMapper.ToObject(data);
        ReturnCode returnCode = (ReturnCode)int.Parse(jsonData["returnCode"].ToString());
        loginPanel.OnLoginResponse(returnCode);//交给UIPanel对象根据返回值做相应处理
        //获取战绩信息
        string username = jsonData["username"].ToString();
        int id = int.Parse(jsonData["id"].ToString());
        int totalCount = int.Parse(jsonData["totalCount"].ToString());
        int winCount=int.Parse(jsonData["winCount"].ToString());

        UserData userData=new UserData(id,username,totalCount,winCount);
        
        gameFacade.SetUserData(userData);//将登陆成功的用户的战绩数据存入PlayerManager中，用于后期其他模块的调用
        
    }

  
}

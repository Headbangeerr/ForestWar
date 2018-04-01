using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class RegisterRequest : BaseRequest {
    private RegisterPanel registerPanel;

    public override void Awake()
    {
        registerPanel = GetComponent<RegisterPanel>();
        this.requestCode = RequestCode.User;//设置自身的RequestCode与ActionCode
        this.actionCode = ActionCode.Register;
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
        JsonData jsonData = new JsonData();
        jsonData["username"] = username;
        jsonData["password"] = password;

        //string data = username + "," + password;//拼接要传送的数据    
        base.SendRequest(JsonMapper.ToJson(jsonData));
    }
    /// <summary>
    /// 服务器端返回响应
    /// </summary>
    /// <param name="data">这里传入的是转成int类型的ReturnCode枚举类型的值</param>
    public override void OnResponse(string data)
    {
        JsonData jsonData = JsonMapper.ToObject(data);
        int returnCodeInt=int.Parse(jsonData["returnCode"].ToString());
        ReturnCode returnCode = (ReturnCode)returnCodeInt;//先将int类型转换为对应的枚举类型
        registerPanel.OnRegisterResponse(returnCode);//交给UIPanel对象根据返回值做相应处理
    }
}

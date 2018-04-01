using System;
using UnityEngine;
/// <summary>
/// 用于解析json字符串时的model对象，属性与json中的名称对应
/// </summary>
[Serializable]
public class UIPanelInfo:ISerializationCallbackReceiver
{
    [NonSerialized]
    public UIPanelType panelType;//JsonUtility自定义类型无法解析，所以解析过程不处理

    public string panelTypeStr;//JsonUtility解析过程不会调用get，set方法，所以下面解决方案不可行
    //{
    //    get { return panelType.ToString(); }//用于解析json过程
    //    set
    //    {
    //        //用于反向生成json字符串时，对string类型属性赋值后，转化为自定义枚举类型
    //        UIPanelType type = (UIPanelType)Enum.Parse(typeof(UIPanelType), value);
    //        panelType = type;//将字符类型转换为枚举类型
    //    }
    //}
    public string panelPath;

    public void OnBeforeSerialize()//序列化之前被调用。序列化：对象->文本信息
    {        
    }

    public void OnAfterDeserialize()//反序列化之后被调用。反序列化：文本信息->对象
    {
        UIPanelType type = (UIPanelType)Enum.Parse(typeof(UIPanelType), panelTypeStr);
        panelType = type;//将字符类型转换为枚举类型
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 对Dictionary类的扩展，需要声明为一个静态类
/// </summary>
public static class DictionaryExtension  {

    /// <summary>
    /// 扩展方法，用于通过key获取value
    /// </summary>
    /// <typeparam name="Tkey">dictionary对象的key的泛型</typeparam>
    /// <typeparam name="Tvalue">dictionary对象的value的泛型</typeparam>
    /// <param name="dict">要操作的dictionary对象</param>
    /// <param name="key">要搜索的键值</param>
    /// <returns></returns>
    public static Tvalue TryGet<Tkey, Tvalue>(this Dictionary<Tkey,Tvalue> dict,Tkey key)
    {
        Tvalue value;
        dict.TryGetValue(key, out value);
        return value;
    }
	
}

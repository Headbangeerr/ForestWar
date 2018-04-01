using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RoleData
{

    public GameObject ExplostionEffect { get;private set; }
    public RoleType RoleType { private set; get; }
    public GameObject RolePrefab { private set; get; }
    public GameObject ArrowPrefab { private set; get; }

    public Vector3 SpawnPosition { private set; get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="roleType">角色类型</param>
    /// <param name="rolePath">角色模型路径</param>
    /// <param name="arrowPath">发射箭矢的模型路径</param>
    /// <param name="spawnPosition">出生位置</param>
    public RoleData(RoleType roleType, string rolePath, string arrowPath,string explosionPath,Transform spawnPosition)
    {
        this.RoleType = roleType;
        this.RolePrefab = Resources.Load<GameObject>(rolePath);
        this.ArrowPrefab = Resources.Load<GameObject>(arrowPath);
        this.SpawnPosition = spawnPosition.position;
        this.ExplostionEffect=Resources.Load(explosionPath) as GameObject;
        ArrowPrefab.GetComponent<ArrowMove>().explosionEffect = ExplostionEffect;

    }
}

using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    public GameObject explosionEffect;
    //是否是本地角色发射的箭矢标志位
    public bool isLocal=false;
    //速度系数
    public int speed = 40;
    //发射箭矢的角色类型
    public RoleType RoleType;
    private Rigidbody rgd;
	// Use this for initialization
	void Start () {
	    rgd = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        rgd.MovePosition(transform.position+ transform.forward * speed * Time.deltaTime);
        //使用Rigidbody对象的MovePosition能够试下更加平滑的移动
		//transform.Translate(Vector3.forward*speed*Time.deltaTime);
	}
    /// <summary>
    /// 箭矢碰撞，播放粒子特效的同时，向服务器发送伤害数值
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameFacade.Instance.PlayNormalSound(AudioManager.Sound_ShootPerson);
            if (isLocal)//判断箭矢本身是否是本地角色发射的
            {
                //判断碰撞到的游戏玩家是否是本地角色
                bool playerIsLocal = other.GetComponent<PlayerInfo>().isLocal;
                //如果碰撞到的角色不是本地的，则代表造成了伤害
                if (playerIsLocal==false)
                {
                    //向服务器发送伤害数值，数值为1-10之间的随机数
                    GameFacade.Instance.SendAttack(Random.Range(1,10));
                    //测试用，一击秒杀
                    //GameFacade.Instance.SendAttack(100);
                }
            }
        }
        else
        {
            GameFacade.Instance.PlayNormalSound(AudioManager.Sound_Miss);
        }
        GameObject.Instantiate(explosionEffect, transform.position, transform.rotation);
        //销毁自身
        GameObject.Destroy(this.gameObject);

    }
}

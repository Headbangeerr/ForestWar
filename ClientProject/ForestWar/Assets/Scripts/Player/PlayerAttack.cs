using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject arrowPrefab;
    private Animator animator;
    private Vector3 shootDir;//射击的方向
    private Transform handTransform;//获取角色模型手部位置

    private PlayerManager playerManager;
	// Use this for initialization
	void Start () {
	    animator = GetComponent<Animator>();
	    handTransform = transform.Find("H_Hips/H_Head");
	}
	
	// Update is called once per frame
	void Update ()
	{
        //获取当前动画状态机是否处于"Grounded"状态
	    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
	    {
	        if (Input.GetMouseButtonDown(0))//鼠标左键点击
	        {
	            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机射出一条物理射线到鼠标点击的点
	            RaycastHit hit;
	            bool isCollider = Physics.Raycast(ray, out hit);//检测物理碰撞
	            if (isCollider)//如果与地面发生碰撞
	            {
                    Vector3 hitPoint = hit.point;//获取碰撞点坐标
                    hitPoint.y = transform.position.y;//保持箭的高度与人物自身的高度一致
                    shootDir = hitPoint - transform.position;//获取射箭的方向
                    transform.rotation=Quaternion.LookRotation(shootDir);//将人物方向朝向射箭的方向
                    animator.SetTrigger("Attack");
                    //延时调用设计方法
                    Invoke("Shoot",0.7f);
	            }
	        }
	    }
	}

    public void SetPlayerManager(PlayerManager playerManager)
    {
        this.playerManager = playerManager;
    }
    /// <summary>
    /// 发射箭矢
    /// </summary>    
    private void Shoot()
    {       
        playerManager.Shoot(arrowPrefab,handTransform.position, Quaternion.LookRotation(shootDir));       
    }
}

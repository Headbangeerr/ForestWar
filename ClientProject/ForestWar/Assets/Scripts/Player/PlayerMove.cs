using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    
    private float speed = 3;//行走的速度
    private Animator animator;

    public float forward = 0;
    void Start () {
        animator = GetComponent<Animator>();
	}

	
	// Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            //任务行动控制
            float h = Input.GetAxis("Horizontal"); //对应摁键：a,d,←，→
            float v = Input.GetAxis("Vertical"); //对应摁键：w,s,↑，↓

            if (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0)//只在摁键时才移动
            {
                transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime, Space.World); //根据摁键方向进行移动，以世界坐标
                transform.rotation = Quaternion.LookRotation(new Vector3(h, 0, v)); //旋转人物一直与摁键方向一致

                forward = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v)); //取得两个方向中数值最大的
                animator.SetFloat("Forward", forward);
            }            
        }
    }
}

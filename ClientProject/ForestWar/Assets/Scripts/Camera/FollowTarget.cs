using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    //摄像机于要跟随物体的相对距离
    Vector3 offset=new Vector3(-0.5195541f, 14.91973f, -17.98522f);
    //要跟随的物体
    public GameObject player;

    private float smoothing = 2;

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = player.transform.position + offset;//摄像机跟随物体的这一帧的目标位置
        //通过插值运算移动到目标位置
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        //摄像机的角度时刻跟随目标物体
        transform.LookAt(player.transform);
    }


    //void Start()
    //{
    //    //获取到摄像机于要跟随物体之间的距离
    //    offset = player.transform.position - transform.position;
    //}
    //// Update is called once per frame
    //void LateUpdate()
    //{
    //    //摄像机的位置
    //    transform.position = Vector3.Lerp(transform.position, player.transform.position - offset, Time.deltaTime * 5);
    //}
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraManager : BaseManager
{
    private GameObject cameraGo;
    private Animator camerAnimator;
  
    private FollowTarget followTarget; //控制摄像机跟随的物体的脚本

    private Vector3 originPosition;//用于保存摄像机的初始位置
    private Vector3 originRotation;//用于保存摄像机的初始角度
    public CameraManager(GameFacade facade) : base(facade)
    {
    }

    
    public override void OnInit()
    {
        cameraGo = Camera.main.gameObject;
        camerAnimator = cameraGo.GetComponent<Animator>();
        followTarget = cameraGo.GetComponent<FollowTarget>();
    }
    /// <summary>
    /// 设置摄像机跟随物体移动
    /// </summary>
    
    public void FollowTarget()
    {
        Debug.Log("FollowTarget");
        //设置跟随脚本的跟随目标，这里通过全局接口获取到当前客户端操作的游戏角色
        followTarget.player = facade.GetCurrentRoleGo();
        //先禁用摄像机的漫游动画，避免给接下来的移动过程造成影响
        camerAnimator.enabled = false;
        //在禁用动画之后，备份当前动画运行最后一帧的摄像机所在位置，以便之后继续播放时能够连贯起来
        originPosition = cameraGo.transform.position;
        originRotation = cameraGo.transform.eulerAngles;
        /*获取到当前摄像机看向目标位置需要旋转的角度
         * 这里使用的LookRotation函数的参数是一个向量，参数的含义是你要看向的方向，这里的
         * 方向是用目标位置（终点）-摄像机位置（起点）的方式得到的
         */
        Quaternion targetQuaternion = Quaternion.LookRotation(followTarget.player.transform.position - cameraGo.transform.position);
        //旋转摄像机角度，看向目标位置
        cameraGo.transform.DORotateQuaternion(targetQuaternion, 1f).OnComplete(delegate()
        {
            //启用跟随脚本
            followTarget.enabled = true;
        });  
    }
    /// <summary>
    /// 当摄像机结束跟随，需要恢复到漫游场景的状态
    /// </summary>
    public void WalkThroughScene()
    {
        //先禁用跟随脚本，防止对动画过程中的位置造成影响
        followTarget.enabled = false;
        //在漫游之前需要先将摄像机移动到动画开始的初始坐标
        cameraGo.transform.DOMove(originPosition, 1f);
        cameraGo.transform.DORotate(originRotation, 1f).OnComplete(delegate()
        {            
            camerAnimator.enabled = true;
            ////不管之前动画播放到哪一帧，都从头开始播放
            //camerAnimator.Play("CameraWander",0,0f);
            //camerAnimator.Update(0f);
        });
    }

}

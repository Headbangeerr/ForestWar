using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    private Text timer;

    private Button successButton;

    private Button failButton;

    private Button quitButton;

    private QuitBattleRequest quitBattleRequest;
    //用于异步显示时间
    private int time = -1;
    void Awake()
    {
        //初始化ui组件
        timer = transform.Find("Timer").GetComponent<Text>();
        successButton= transform.Find("SuccessButton").GetComponent<Button>();
        successButton.gameObject.SetActive(false);
        failButton = transform.Find("FailButton").GetComponent<Button>();
        failButton.gameObject.SetActive(false);
        quitButton = transform.Find("QuitButton").GetComponent<Button>();
        quitButton.gameObject.SetActive(false);
        //添加结果摁钮的点击事件
        successButton.onClick.AddListener(OnResultClick);
        failButton.onClick.AddListener(OnResultClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
        //获取中途退出请求对象
        quitBattleRequest = GetComponent<QuitBattleRequest>();

    }

    void Update()
    {
        //异步更新时间
        if (time>-1)
        {
            ShowTime(time);
            time = -1;
        }
    }
    /// <summary>
    /// 显示倒计时
    /// </summary>
    /// <param name="time"></param>
    public void ShowTime(int time)
    {
        //计时结束，进入游戏后，显示退出游戏摁钮
        if (time==1)
        {
            quitButton.gameObject.SetActive(true);
        }

        timer.gameObject.SetActive(true);
        timer.text = time.ToString();
        //首先初始化字体的颜色与大小
        timer.transform.localScale = new Vector2(1, 1);
        Color tempColor = timer.color;
        tempColor.a = 1;
        timer.color = tempColor;
        //播放特效动画
        timer.transform.DOScale(2, 0.3f).SetDelay(0.3f);//先放大，延后延迟0.3秒
        timer.DOFade(0, 0.3f).SetDelay(0.3f).OnComplete(()=>timer.gameObject.SetActive(false));//延迟0.3秒后消失
        facade.PlayNormalSound(AudioManager.Sound_Alert);
        
    }
    /// <summary>
    /// 异步显示倒计时时间
    /// </summary>
    /// <param name="time"></param>
    public void ShowTimeSync(int time)
    {
        this.time = time;
    }
    /// <summary>
    /// 游戏过程中中途退出摁钮
    /// </summary>
    private void OnQuitButtonClick()
    {
        quitBattleRequest.SendRequest();
    }

    /// <summary>
    /// 处理游戏结束后，结果摁钮的点击事件
    /// </summary>
    private void OnResultClick()
    {
        //弹出游戏结果面板自身
        uiManager.PopPanel();
        //弹出房间面板，回到房间列表面板
        uiManager.PopPanel();
        //游戏结束处理
        facade.GameOver();
    }
    /// <summary>
    /// 对中途退出的响应
    /// </summary>
    public void OnQuitResponse()
    {
        OnResultClick();
    }
    /// <summary>
    /// 处理游戏结束响应，显示游戏结果
    /// </summary>
    /// <param name="returnCode"></param>
    public void OnGameOverResponse(ReturnCode returnCode)
    {
        Button tempBtn = null;
        //根据战斗结果显示不同的结果摁钮
        switch (returnCode)
        {
            case ReturnCode.Success:
                tempBtn = successButton;
                break;
            case ReturnCode.Fail:
                tempBtn = failButton;
                break;
        }
        tempBtn.gameObject.SetActive(true);
        tempBtn.transform.localScale = Vector3.zero;
        tempBtn.transform.DOScale(1, 0.5f);
    }

    public override void OnEnter()
    {
        gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        //禁用面板的ui物体
        successButton.gameObject.SetActive(false);
        failButton.gameObject.SetActive(false);        
        quitButton.gameObject.SetActive(false);
        //先禁用子物体，再禁用父物体
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : BaseManager {
    //方便后期修改，将所有音频文件的名称以静态字符串的形式进行保存
    private const string Path_Pre = "Sounds/";
    public const string Sound_Alert = "Alert";
    public const string Sound_ArrowShoot = "ArrowShoot";
    public const string Sound_Bg_Fast = "Bg(fast)";
    public const string Sound_Bg_Moderate = "Bg(moderate)";
    public const string Sound_ButtonClick = "ButtonClick";
    public const string Sound_Miss = "Miss";
    public const string Sound_ShootPerson = "ShootPerson";
    public const string Sound_Timer = "Timer";

    private AudioSource bgAudioSource;//只用于播放背景音乐的AudioSource组件
    private AudioSource normalAudioSource;//用于播放其他音乐的AudioSource组件

    public override void OnInit()
    {
        base.OnInit();
        //创建一个空物体，用于添加AudioSource组件，播放音频
        GameObject audioSourceGO = new GameObject("AudioSource(GameObject)");
        //为空物体添加AudioSource组件
        bgAudioSource = audioSourceGO.AddComponent<AudioSource>();
        normalAudioSource = audioSourceGO.AddComponent<AudioSource>();

        PlaySound(bgAudioSource,LoadSound(Sound_Bg_Moderate),0.2f,true);//循环播放正常速度背景音乐
        
    }
    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="audioClip">要播放的音频文件</param>
    /// <param name="loop">是否循环播放，默认为不循环</param>
    private void PlaySound(AudioSource audioSource,AudioClip audioClip, float volume,bool loop=false)
    {
        audioSource.volume = volume;
        audioSource.clip = audioClip;//获取正常速度的Bg音乐
        audioSource.loop = loop;//循环播放
        audioSource.Play();//播放音乐
    }
    /// <summary>
    /// 循环播放指定的背景音乐
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayBgSound(string soundName)
    {
        PlaySound(bgAudioSource,LoadSound(soundName),0.2f,true);
    }
    /// <summary>
    /// 播放指定的一般音效
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayNormalSound(string soundName)
    {

        PlaySound(normalAudioSource,LoadSound(soundName),1);
    }
    /// <summary>
    /// 通过音频文件名获取音频资源
    /// </summary>
    /// <param name="soundName"></param>
    /// <returns></returns>
    private AudioClip LoadSound(string soundName)
    {
        return Resources.Load<AudioClip>(Path_Pre + soundName);
    }
    public AudioManager(GameFacade facade) : base(facade)
    {
    }
}

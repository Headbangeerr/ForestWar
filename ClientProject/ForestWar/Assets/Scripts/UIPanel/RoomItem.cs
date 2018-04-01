using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{

    public Text username;

    public Text totalCount;

    public Text winCount;

    public Button joinButton;

    private int id;//房间id

    private RoomListPanel roomListPanel;
	// Use this for initialization
	void Start () {
	    if (joinButton!=null)
	    {
	        joinButton.onClick.AddListener(OnJoinButtonClick);
	    }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 设置房间列表中一个条目的房间信息
    /// </summary>
    /// <param name="username"></param>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    public void SetRoomItemInfo(int id,string username,int totalCount,int winCount,RoomListPanel roomListPanel)
    {
        this.id = id;
        this.roomListPanel = roomListPanel;
        this.username.text = username;
        this.totalCount.text = "总场数："+totalCount;
        this.winCount.text = "胜场："+winCount;
    }
    /// <summary>
    /// 加入摁钮点击触发事件
    /// </summary>
    private void OnJoinButtonClick()
    {
        //通过向RoomListPanel传入自身的id来加入指定的房间
        roomListPanel.OnJoinButtonClick(this.id);
    }
    /// <summary>
    /// 销毁自身游戏物体
    /// </summary>
    public void DestorySelf()
    {
        GameObject.Destroy(this.gameObject);
    }

}

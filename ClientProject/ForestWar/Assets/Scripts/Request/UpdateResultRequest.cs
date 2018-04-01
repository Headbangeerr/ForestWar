using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class UpdateResultRequest : BaseRequest
{
    private RoomListPanel roomListPanel;
    private bool isUpdate=false;
    private int totalCount;
    private int winCount;
    public override void Awake()
    {
        roomListPanel = GetComponent<RoomListPanel>();
        this.actionCode = ActionCode.UpdateResult;
        base.Awake();
    }

    void Update()
    {
        if (isUpdate)
        {
            //异步更新房间列表中的用户战绩信息
            roomListPanel.OnUpdateResultResponse(totalCount, winCount);
            isUpdate = false;
        }
    }
  
    public override void OnResponse(string data)
    {
        JsonData jsonData = JsonMapper.ToObject(data);
        //获取到最新的战绩信息
        totalCount = int.Parse(jsonData["totalCount"].ToString());
        winCount = int.Parse(jsonData["winCount"].ToString());
        isUpdate = true;
    }
}

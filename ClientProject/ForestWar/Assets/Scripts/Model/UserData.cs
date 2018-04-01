using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData {

    public UserData()
    {
        
    }
    public UserData(int id,string username,int totalCount,int winCount)
    {
        this.Id = id;
        this.Username = username;
        this.TotalCount = totalCount;
        this.WinCount = winCount;
    }

    public int Id { set; get; }
    public string Username { set; get; }
    public int TotalCount {  set; get; }
    public int WinCount {  set; get; }


}

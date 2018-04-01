

namespace Common
{
    public enum ActionCode
    {
        None,
        Login,//用户登录
        Register,//用户注册
        ListRoom,//展示房间列表
        CreateRoom,//创建房间
        JoinRoom,//加入房间
        UpdateRoom,//有新的玩家进入房间
        QuitRoom,//玩家退出房间
        StartGame,//开始游戏
        ShowTimeer,//显示计时器
        StartPlay,//开始游戏
        Move,//角色移动
        Shoot,//设计箭矢
        Attack,//箭矢命中敌人            
        GameOver,//游戏结束
        UpdateResult,//更新用户战绩信息
        QuitBattle//中途退出战斗
    }
}

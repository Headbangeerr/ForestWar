using System;
using System.Collections;
using System.Collections.Generic;
using Assets.UIFreamwork.UIPanel;
using UnityEngine;

public class UIManager:BaseManager
{
     
    // 单例模式的核心：定义一个静态对象，用于被外接访问，在内部构造；内部构造私有化
     
    //private static UIManager _instance;//单例模式唯一实例
    //用于管理所有UI面板类型的字典，键为面板类型，值为UI面板预制体所在路径
    private Dictionary<UIPanelType,string> panelPathDic;
    private Dictionary<UIPanelType,BasePanel> panelDic;//用于保存所有面板实例对象
    private Stack<BasePanel> panelStack;//用于控制面板显示的栈
    private Transform canvasTransform;//画布对象
    private MessagePanel messagePanel;//提示面板对象
    private UIPanelType panelTypeTooPush=UIPanelType.None;//用于异步入栈UI面板对象
    /// <summary>
    /// 初始化操作
    /// </summary>
    public override void OnInit()
    {
        base.OnInit();
        PushPanel(UIPanelType.Message);
        PushPanel(UIPanelType.Start);//默认加载开始面板和消息面板
       
    }
    /// <summary>
    /// 帧更新方法
    /// </summary>
    public override void Update()
    {
        //通过Update方法来实现异步调用PushPanel
        if (panelTypeTooPush!=UIPanelType.None)
        {
            PushPanel(panelTypeTooPush);
            panelTypeTooPush=UIPanelType.None;
        }

    }

    /// <summary>
    /// 注入MessagePanel对象，在MessagePanel对象实例化时，将自身实例注入给UIManager，用于给其他面板对象使用，用于显示提示信息
    /// </summary>
    public void InjectMessagePanel(MessagePanel messagePanel)
    {
        this.messagePanel = messagePanel;
    }
    private Transform CanvasTransform//获取Canvas对象
    {
        get
        {
            if (canvasTransform==null)
            {
                canvasTransform = GameObject.Find("Canvas").transform;
            }
            return canvasTransform;
        }
        
    }

    public UIManager(GameFacade facade) : base(facade)
    {
        ParseUIPanelTypeJson();
    }
    //public UIManager()//单例模式需要将构造函数私有化
    //{
    //    //_instance = this;
    //    ParseUIPanelTypeJson();
    //}


    /// <summary>
    /// 将面板进行入栈操作，并显示出来
    /// </summary>
    /// <param name="panelType"></param>
    /// <returns>成功入栈的面板实例</returns>
    public BasePanel PushPanel(UIPanelType panelType)
    {
        Debug.Log("pushPanel:"+panelType);
        if (panelStack==null)//第一次调用时先初始化栈
        {
            panelStack=new Stack<BasePanel>();
        }
        if (panelStack.Count > 0)//如果栈不为空，则先将栈顶元素暂停，再入栈新的面板
        {
            BasePanel topPanel = panelStack.Peek();//获取栈顶元素
            topPanel.OnPause();//将栈顶元素的面板对象暂停
        }

        BasePanel panel = GetPanel(panelType);//获取面板实例
        panel.OnEnter();
        panelStack.Push(panel);//将获取到的实例入栈，并显示
        return panel;
    }
    /// <summary>
    /// 通过Update方法异步入栈Ui面板
    /// </summary>
    /// <param name="panelType"></param>
    public void PushPanelSync(UIPanelType panelType)
    {
        //将私有变量赋值，然后通过update方法中检验到值被修改，在主线程的Update方法中调用PushPanel
        this.panelTypeTooPush = panelType;
    }
    /// <summary>
    /// 将面板进行出栈，并在界面中关闭面板
    /// </summary>
    public void PopPanel()
    {
        if (panelStack == null)
        {
            panelStack=new Stack<BasePanel>();
        }
        if (panelStack.Count<=0)//如果栈为空，则直接返回
        {
            return;
        }
        BasePanel topPanel = panelStack.Pop();//将栈顶元素出栈
        Debug.Log("Pop:"+topPanel.name);
        topPanel.OnExit();
        
        if (panelStack.Count>0)//如果栈内不为空，则重新激活栈顶元素
        {
            topPanel = panelStack.Peek();
            topPanel.OnResume();
            Debug.Log("被激活的面板："+topPanel.name);

        }
        else//如果栈为空则直接返回
        {
            return;            
        }
    }
    /// <summary>
    /// 将面板进行出栈，并在界面中关闭面板
    /// </summary>
    public void PopPanel(BasePanel panel)
    {
        if (panelStack == null)
        {
            panelStack = new Stack<BasePanel>();
        }
        if (panelStack.Count <= 0)//如果栈为空，则直接返回
        {
            return;
        }
        BasePanel topPanel;
        if (panel == panelStack.Peek())
        {
            topPanel = panelStack.Pop();//将栈顶元素出栈
            Debug.Log("Pop:" + topPanel.name);
            topPanel.OnExit();
        }       
        if (panelStack.Count > 0)//如果栈内不为空，则重新激活栈顶元素
        {
            topPanel = panelStack.Peek();
            topPanel.OnResume();
            Debug.Log("被激活的面板：" + topPanel.name);

        }
        else//如果栈为空则直接返回
        {
            return;
        }
    }

    /// <summary>
    /// 通过面板类型获取对应的面板实例对象
    /// </summary>
    /// <param name="panelType"></param>
    /// <returns></returns>
    private BasePanel GetPanel(UIPanelType panelType)
    {
        if (panelDic == null)//第一次调用，先实例化字典对象
        {
            panelDic = new Dictionary<UIPanelType, BasePanel>();
        }
        //BasePanel panel;
        //panelDic.TryGetValue(panelType, out panel);//通过panelType获取实例对象
        BasePanel panel = panelDic.TryGet(panelType);

        if (panel == null) //如果为空，则代表这个类型的panel对象还没有添加到字典中
        {
            string panelPath=panelPathDic.TryGet(panelType); //先通过panel类型获取到其预制体在项目中的所在路径

            GameObject instancePanel = GameObject.Instantiate(Resources.Load(panelPath)) as GameObject; //通过路径加载预制体并实例化
            instancePanel.transform.SetParent(CanvasTransform,false); //将实例化的面板对象添加至画布中
            /*
             * ****************注意：上述方法的第二个参数十分重要***************
             * 第二个参数的含义是是否保持世界坐标。由于ui预制体是在世界坐标系下进行初始化的，因此它的大小与位置是相对世界坐标系的
             * 再通过setParent方法设置父级对象时，会出现缩放比例以及位置的变化，从而出现ui元素位置偏移显示错误的问题，
             * 所以这里的第二个参数需要设置为false，即表示设置父级对象的同时，按照ui元素自身的位置与缩放进行显示
             */
            instancePanel.GetComponent<BasePanel>().UIManager = this;//在对所有面板进行初始化时，将自身实例赋给各个面板
            instancePanel.GetComponent<BasePanel>().Facade = facade;//将自己从父类中获得的GameFacade对象赋值给各个面板
            panelDic.Add(panelType, instancePanel.GetComponent<BasePanel>()); //将新实例化的面板添加至字典中
            return instancePanel.GetComponent<BasePanel>();
        }
        else//成功获取实例，直接返回
        {
            return panel;
        }
    }
    /// <summary>
    /// 使用异步方式显示提示信息
    /// </summary>
    /// <param name="msg"></param>
    public void ShowMessageSync(string msg)
    {
        if (messagePanel == null)
        {
            Debug.Log("MessagePanel对象为空");
            return;
        }
        messagePanel.ShowMessageSync(msg);
    }
    /// <summary>
    /// 显示提示面板
    /// </summary>
    public void ShowMessage(string msg)
    {
        if (messagePanel == null)
        {
            Debug.Log("MessagePanel对象为空");
            return;           
        }
        messagePanel.ShowMessage(msg);
    }
    /// <summary>
    /// 提供唯一的外接访问实例
    /// </summary>
    //public static UIManager GetInstance
    //{
    //    get
    //    {
    //        if (_instance==null)
    //        {
    //            _instance= new UIManager();                
    //        }
    //        return _instance;
    //    }
    //}
   
    /// <summary>
    /// 解析json文件，将所有面板信息保存至字典中
    /// </summary>
    public void ParseUIPanelTypeJson()
    {
        if (panelPathDic == null)
        {
            panelPathDic =new Dictionary<UIPanelType, string>();
        }
        //TextAsset是unity中文本文件资源类型
        TextAsset ta = Resources.Load<TextAsset>("UIPanelType");//在Resources目下搜索指定名称的文件，不需要指定文件后缀        
        UIPanelInfoJson infoJson = JsonUtility.FromJson<UIPanelInfoJson>(ta.text);//通过Unity的json解析工具JsonUtility对文件进行解析，并转化为List集合
        foreach (UIPanelInfo uiPanelInfo in infoJson.panelInfoList)//将解析出来的list集合中的panel信息添加至字典中
        {
            //Debug.Log(uiPanelInfo.panelType);
            panelPathDic.Add(uiPanelInfo.panelType,uiPanelInfo.panelPath);
        }
    }

    //public  void Test()
    //{        
    //    string path;        
    //    panelPathDic.TryGetValue(UIPanelType.ItemInfo, out path);
    //    Debug.Log(path);
    //}
}

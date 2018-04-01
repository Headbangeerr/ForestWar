using System;
using System.Collections.Generic;
using System.Reflection;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    /// <summary>
    /// 用于管理所有Controller的控制类，也是所有Controller的对外接口，所有Controller的调用都集成自该类
    /// </summary>
    class ControllerManager
    {
        //一个RequestCode对应一种类型的Controller，是用字典类型便于管理
        private  Dictionary<RequestCode,BaseController> controllerDict = new Dictionary<RequestCode,BaseController>();

        private Server server;//服务器对象
        public ControllerManager(Server server)
        {
            this.server = server;//通过构造方法获得Server对象
            InitControllers();
        }
        /// <summary>
        /// 初始化所有Controller
        /// </summary>
        private void InitControllers()
        {
            //TODO 根据RequestCode初始化各种Controller
            DefaultController defaultController = new DefaultController();
            controllerDict.Add(defaultController.RequestCode,defaultController);//将默认Controller添加至管理字典中
            controllerDict.Add(RequestCode.User, new UserController());//添加新实例化的UserController
            controllerDict.Add(RequestCode.Room, new RoomController());//添加新实例化的RoomController
            controllerDict.Add(RequestCode.Game, new GameController());//添加新实例化的GameController
        }
        /// <summary>
        /// 根据RequestCode与ActionCode，来调用指定Controller中的指定处理方法
        /// </summary>
        /// <param name="data">请求中传递的数据</param>
        /// <param name="requestCode">请求编号</param>
        /// <param name="actionCode">请求的方法编号</param>
        /// <param name="client">客户端连接对象</param>
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode,string data,Client client)
        {
            BaseController controller;
            bool isGet = controllerDict.TryGetValue(requestCode, out controller);//通过RequestConde获取到对应的Controller对象，这里是父类对象指向子类引用
            if (isGet==false)//获取Controller失败
            {
                Console.WriteLine("无法获取["+requestCode+"]所对应的Controller！");return;
            }
            string methodName = Enum.GetName(typeof(ActionCode),actionCode);//通过枚举类型的值获取到对应值的名称

            //Console.WriteLine("客户端发起了["+ requestCode+"]类型的请求，请求的方法是["+methodName+"]。");
            MethodInfo methodInfo= controller.GetType().GetMethod(methodName);//利用反射机制，通过方法名称获取到Controller中对应的方法信息
            if (methodInfo==null)
            {
                Console.WriteLine("[警告]：在" + controller.GetType() +"中没有找到名为["+methodName+"]的处理方法");
                return;
            }
            Object[] parameters=new object[]{data,client,server};

            //通过MethodInfo对象进行函数调用，第一个参数为调用函数的所属对象，第二个参数为要传递的函数形参，返回值为调用方法的返回值
            Object returnObject =methodInfo.Invoke(controller, parameters);
            if (returnObject == null || string.IsNullOrEmpty(returnObject as string))//判断anction方法响应请求以后的返回值是否为空，或者是否为空字符串
            {
                return;
            }
            server.SendResponse(client,actionCode,returnObject as string);//通过服务器端发送响应结果            
        }

    }
}

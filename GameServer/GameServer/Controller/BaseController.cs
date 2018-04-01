using System;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    /// <summary>
    /// Controller基类，所有的Controller类都继承自该类
    /// </summary>
    abstract class BaseController
    {
        protected RequestCode requestCode=RequestCode.None;
        /// <summary>
        /// 用于外接接口获取RequestCode属性
        /// </summary>
        public RequestCode RequestCode
        {
            get { return requestCode; }
        }

        /// <summary>
        /// 默认处理方法，为虚函数，子类可以选择性重写
        /// </summary>
        /// <param name="data">客户端发送的数据</param>
        /// <returns>相应信息</returns>
        public virtual string DefaultHandle(string data,Client client,Server server)
        {
            return null;
        }

    }
}

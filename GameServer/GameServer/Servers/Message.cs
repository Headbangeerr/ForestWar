using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace GameServer.Servers
{
    class Message
    {
        private byte[] data = new byte[1024];//用于存储来自客户端的数据包的字节数组
        private int startIndex = 0;//读取标志位，默认为零，即表示从第0个字节开始读取字节数组，也表示该数组中已经存储了多少个字节的数据
        /*
         *    |  *   *   *   *  |*RequestCode*|*ActionCode* |---------------------------------|
         *    | 数据长度标志位  |请求编号(int)|方法编号(int)|       请求中的数据内容          |
         *    
         *        每次发送的数据包格式如上所示，前四个字节始终用于保存一个int32类型的数据，该数据为数据长度标志位，即数据内容
         *    部分所占的字节数。使用这样格式传输数据的目的是为了解决TCP协议中的粘包问题，高频率发送小体量数据包会出现多个数据包合并发送的
         *    情况，也就是说一次接收可能会接收到多个数据包拼接而成的一个大自己数组。
         *        因此，就需要一个数据长度标志位来告知服务器端，如何将一个大的数据包进行分解。通过数据长度标志位就可以知道
         *    一个小数据包的长度，这样服务器端就可以根据长度对大数据包进行分解解析。         
         */
        public byte[] Data
        {
            get { return data; }
        }

        public int StartIndex
        {
            get { return startIndex; }
        }

        public int ResidueSize//数据数组剩余的空间
        {
            get { return data.Length - startIndex; }
        }

        //public void AddCount(int count)
        //{
        //    startIndex += count;
        //}

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="newDataAmount">读入数据的字节数</param>
        public void ReadMessage(int newDataAmount,Action<RequestCode,ActionCode,string> processDataCallBack)
        {
            startIndex += newDataAmount;//更新数据长度           
            while (true)//循环处理数据
            {
                if (startIndex <= 4) return;//如果接收的数据本身长度不大于4，则代表前4位的int类型的数据长度可能都没有保存完整
                int dataLength = BitConverter.ToInt32(data, 0);//读入data数据，从秩为0的位置开始，读入4个字节的int32类型
               
                if ((startIndex - 4) >= dataLength) //如果已读入数据的总长度减去前4位，剩余的长度不小于字节数组中存储的int类型的长度标志位
                {
                    RequestCode requestCode = (RequestCode)BitConverter.ToInt32(data, 4);//通过强转，从秩为4的位置获取RequestCode
                    ActionCode actionCode = (ActionCode) BitConverter.ToInt32(data, 8);//获取请求方法编号
                    string dataStr = Encoding.UTF8.GetString(data,12, dataLength - 8);//从秩为12的位置开始获取请求中的数据
                    processDataCallBack(requestCode, actionCode, dataStr);//通过回调函数对解析出来的请求数据作进一步的处理
                    Array.Copy(data, dataLength + 4, data, 0,
                        (startIndex - dataLength - 4)); //成功解析一条数据之后，将未解析部分向前移动，并覆盖掉已经解析完成的部分
                    startIndex -= (dataLength + 4);//由于已经将已读取的数据移除了，所以需要更新data数组的整体长度
                }
                else//如果已经接收的数据不满足数据包中长度标志位的长度，则表示数据包被拆分了，此时数据包是不完整的，因此直接退出循环，等待下次接收到数据，再通过ReadMessage方法进行读取
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 包装响应数据
        /// </summary>
        /// <param name="actionCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] PackData(ActionCode actionCode, string data)
        {
            //将要包装的数据全部转换为字节数组
            byte[] actionCodeBytes = BitConverter.GetBytes((int) actionCode);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int dataLength = dataBytes.Length + actionCodeBytes.Length;
            byte[] dataLengthBytes = BitConverter.GetBytes(dataLength);            
            //将转换后的数据按照数据包：【数据总长度+RequestCode+数据】的格式拼接
            byte[] packageBytes = dataLengthBytes.Concat(actionCodeBytes).Concat(dataBytes).ToArray(); 
            return packageBytes;
        }

    }
}

using Common;
using GameServer.DAO;
using GameServer.Model;
using GameServer.Servers;
using LitJson;

namespace GameServer.Controller
{
    class UserController:BaseController
    {
        private UserDAO userDao=new UserDAO();//获取数据库操作类
        private ResultDAO resultDao=new ResultDAO();
        /// <summary>
        /// 默认构造方法
        /// </summary>
        public UserController()
        {
            requestCode=RequestCode.User;//设置自身的RequestCode
        }
        /// <summary>
        /// 处理登陆请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        public string Login(string data, Client client, Server server)
        {
            //根据客户端传入的数据格式，拆分数据
            //string[] datas = data.Split(',');
            //string username = datas[0];
            //string password = datas[1];

            //通过LitJson插件解析Json
            JsonData jsonData = JsonMapper.ToObject(data);
            string username = jsonData["username"].ToString();
            string passwrod = jsonData["password"].ToString();
            //通过Dao层对象验证用户信息
            User user=userDao.VerifyUser(client.MySqlConnection, username, passwrod);

            jsonData = new JsonData();
            if (user == null)//验证失败，用户名或密码错误
            {
                jsonData["returnCode"] = ((int) ReturnCode.Fail).ToString();
                return JsonMapper.ToJson(jsonData);
            }
            else//验证成功，
            {
                //通过用户id获取其战绩信息
                Result result = resultDao.GetResultByUserId(client.MySqlConnection, user.Id);
                jsonData["id"] = user.Id;
                jsonData["returnCode"] = ((int) ReturnCode.Success).ToString();
                jsonData["username"] = user.Username;
                jsonData["totalCount"] = result.TotalCount;
                jsonData["winCount"] = result.WinCount;
                UserData userData=new UserData(user.Id,user.Username,result.TotalCount,result.WinCount);
                //将成功登录的用户信息存入连接对象中
                client.SetUserData(userData);
                return JsonMapper.ToJson(jsonData);
            }
        }
        /// <summary>
        /// 处理注册请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public string Register(string data, Client client, Server server)
        {
            //根据客户端传入的数据格式，拆分数据
            //string[] datas = data.Split(',');
            //string username = datas[0];
            //string password = datas[1];
            JsonData jsonData = JsonMapper.ToObject(data);//解析json字符串
            string username = jsonData["username"].ToString();
            string password = jsonData["password"].ToString();
            //通过usernanme获取用户
            User temp = userDao.GetUserByUsername(client.MySqlConnection, username);
            jsonData=new JsonData();
            if (temp==null)//获取失败，则代表该用户名在数据库不存在，可以注册
            {
                if (userDao.AddUesr(client.MySqlConnection, username, password))//添加用户记录成功
                {
                    jsonData["returnCode"]= ((int)ReturnCode.Success).ToString();                  
                    //return ((int)ReturnCode.Success).ToString();
                }
                else//添加失败
                {
                    jsonData["returnCode"] = ((int)ReturnCode.Fail).ToString();
                    //return ((int)ReturnCode.Fail).ToString();
                }
            }
            else//用户名重复，注册失败
            {
                jsonData["returnCode"] = ((int)ReturnCode.Fail).ToString();
                //return ((int)ReturnCode.Fail).ToString();
            }
            return JsonMapper.ToJson(jsonData);
        }
    }
}

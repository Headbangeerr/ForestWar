using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    class UserDAO
    {
        /// <summary>
        /// 查询数据库验证用户信息
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public User VerifyUser(MySqlConnection connection,string username,string password)
        {
            MySqlDataReader reader=null;
            try
            {
                //创建mysql操作句柄，创建查询语句
                MySqlCommand cmd =
                    new MySqlCommand("select * from user where username=@username and password=@password", connection);
                //赋值查询参数
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                reader = cmd.ExecuteReader(); //执行查询语句，并获取Reader对象用于读取查询结果
                if (reader.Read()) //如果查询结果不为空
                {
                    int id = reader.GetInt32("id"); //获取到user记录的id
                    User user = new User(id, username, password); //由于能够成功查询，所以传入的参数与数据库中的已知，所以直接赋值即可
                    return user;
                }
                else //如果查询结果为空
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("用户登录验证数据库操作抛出异常："+e);
            }
            finally
            {
                //最终一定要关闭Reader对象，否则会造成资源浪费
                if (reader!=null)
                {
                    reader.Close();
                }                
            }
            return null;
        }
        /// <summary>
        /// 通过用户名获取一个User对象
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="username"></param>
        /// <returns>获取失败返回值为null，获取成功直接返回User对象</returns>
        public User GetUserByUsername(MySqlConnection connection, string username)
        {
            MySqlDataReader reader = null;
            try
            {
                //创建mysql操作句柄，创建查询语句
                MySqlCommand cmd =
                    new MySqlCommand("select * from user where username=@username", connection);
                //赋值查询参数
                cmd.Parameters.AddWithValue("username", username);               
                reader = cmd.ExecuteReader(); //执行查询语句，并获取Reader对象用于读取查询结果
                if (reader.Read()) //如果查询结果不为空
                {
                    int id = reader.GetInt32("id"); //获取到user记录的id
                    string password = reader.GetString("password");
                    User user = new User(id, username, password); //由于能够成功查询，所以传入的参数与数据库中的已知，所以直接赋值即可
                    return user;
                }
                else //如果查询结果为空
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("验证用户是否存在数据库操作抛出异常：" + e);
            }
            finally
            {
                //最终一定要关闭Reader对象，否则会造成资源浪费
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return null;
        }

        public bool AddUesr(MySqlConnection connection,string username,string password)
        {           
            try
            {
                //创建mysql操作句柄，创建查询语句
                MySqlCommand cmd =
                    new MySqlCommand("insert into user set username=@username , password=@password", connection);
                //赋值查询参数
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                int count=cmd.ExecuteNonQuery();
                if (count==1) //如果查询结果不为空
                {                   
                    return true;
                }
                else //如果插入失败
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("用户登录验证数据库操作抛出异常：" + e);
            }          
            return false;
        }
    }
}

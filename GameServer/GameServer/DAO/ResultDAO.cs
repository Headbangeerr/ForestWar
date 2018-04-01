using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    class ResultDAO
    {
        /// <summary>
        /// 通过用户id查询用户战绩
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userId"></param>
        /// <returns>不管查找成功与否，都会返回一个Result对象。如果数据库中存在记录则正常返回记录，
        /// 如果没有记录，则返回一个id为-1的Result对象，并且其中的战绩信息均为零</returns>
        public Result GetResultByUserId(MySqlConnection connection, int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                //创建mysql操作句柄，创建查询语句
                MySqlCommand cmd = new MySqlCommand("select * from result where user_id=@userid", connection);
                //赋值查询参数
                cmd.Parameters.AddWithValue("userid", userId);
                reader = cmd.ExecuteReader(); //执行查询语句，并获取Reader对象用于读取查询结果
                if (reader.Read()) //如果查询结果不为空
                {
                    int id = reader.GetInt32("id");
                    int totalCount = reader.GetInt32("total_count");
                    int winCount = reader.GetInt32("win_count");
                    Result result = new Result(id, userId, totalCount, winCount);
                    return result;
                }
                else //如果查询结果为空
                {
                    //查询战绩是在用户已经成功登陆以后才能操作的，所以这里查询不到则代表该用户第一次参加游戏，所以这里默认为0
                    Result result = new Result(-1, userId, 0, 0);
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("查询用户战绩数据库操作抛出异常：" + e);
            }
            finally
            {
                //最终一定要关闭Reader对象，否则会造成资源浪费
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return new Result(-1, userId, 0, 0); ;
        }
        /// <summary>
        /// 更新用户战绩信息
        /// </summary>
        /// <param name="userData"></param>
        public void UpdateResult(MySqlConnection connection,UserData userData)
        {
            MySqlDataReader reader = null;
            try
            {
                //创建mysql操作句柄，创建查询语句
                MySqlCommand cmd = null;
                Result result = GetResultByUserId(connection,userData.Id);//先查询该用户是否有战绩记录
                if (result.Id==-1)//如果数据库中没有找到记录，则添加
                {
                    cmd = new MySqlCommand("insert into result set total_count=@totalCount," +
                                           "win_count=@winCount,user_id=@userId",connection);                    
                }
                else//如果找到记录，则更新
                {
                    cmd = new MySqlCommand("update result set total_count=@totalCount," +
                                           "win_count=@winCount where user_id=@userId", connection);
                    
                }
                cmd.Parameters.AddWithValue("totalCount", userData.TotalCount);
                cmd.Parameters.AddWithValue("winCount", userData.WinCount);
                cmd.Parameters.AddWithValue("userId", userData.Id);
                //执行sql语句
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("查询用户战绩数据库操作抛出异常：" + e);
            }
        }
    }
}

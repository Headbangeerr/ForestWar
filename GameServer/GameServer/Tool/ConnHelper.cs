using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GameServer.Tool
{
    class ConnHelper
    {
        //常量类型数据库连接字符串
        public const string CONNECTIONSTR="datasource=127.0.0.1;port=3306;database=forest_war;user=root;pwd=123456;";
        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <returns>成功打开的数据库连接对象</returns>
        public static MySqlConnection Connect()
        {
            MySqlConnection connection=new MySqlConnection(CONNECTIONSTR);//创建数据库连接
            try//防止数据库连接抛出异常中断
            {
                connection.Open();
                return connection;
            }
            catch (Exception e)
            {
                Console.WriteLine("数据库连接失败抛出异常："+e);
                return null;
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="connection"></param>
        public static void CloseConnection(MySqlConnection connection)
        {
            if (connection != null)
            {
                connection.Close();
            }
            else
            {
                Console.WriteLine("[警告]要关闭的连接对象为空！");
            }           
        }
    }
}

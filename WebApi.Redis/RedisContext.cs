using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Redis
{
    public class RedisContext
    {
        private readonly string m_ConnectionString;
        public RedisContext(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            m_ConnectionString = configuration["ConnectionStrings:RedisServerUrl"];
        }
        private StackExchange.Redis.ConnectionMultiplexer GetConnection()
        {
            var connection = StackExchange.Redis.ConnectionMultiplexer.Connect(m_ConnectionString);
            return connection;
        }

        /// <summary>
        /// 向List插入数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool SetList(string key, string value, TimeSpan? expiry = null)
        {
            using (var connection = GetConnection())
            {
                var db = connection.GetDatabase();
                db.ListLeftPush(key, value);

                return true;
            }
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool ListRemove(string key,string value)
        {
            using (var connection = GetConnection())
            {
                var db = connection.GetDatabase();
                db.ListRemove(key, value);

                return true;
            }
        }

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="oldvalue">旧的值</param>
        /// <param name="value">新的值</param>
        /// <returns></returns>
        public bool ListSetByIndex(string key,string oldvalue,string value)
        {
            using (var connection = GetConnection())
            {
                var db = connection.GetDatabase();
                var length = db.ListLength(key);
                for (var i = 0; i < length; i++)
                {
                    var valueIndex = db.ListGetByIndex(key, i);
                    if(valueIndex == oldvalue)
                    {
                        db.ListSetByIndex(key, i, value);
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 读取List中的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> GetList(string key)
        {
            var list = new List<string>();
            using (var connection = GetConnection())
            {
                var db = connection.GetDatabase();
                var len = db.ListLength(key);
                for (var i = 0; i < len; i++)
                {
                    list.Add(db.ListGetByIndex(key, i).ToString());
                }
            }
            return list;
        }

        /// <summary>
        /// 设置数据超时的时间
        /// </summary>
        /// <param name="keyName">键</param>
        /// <param name="expiry">超时时间</param>
        public void SetTimeout(string keyName, TimeSpan expiry)
        {
            using (var connection = GetConnection())
            {
                var db = connection.GetDatabase();
                var result = db.KeyExpire(keyName, expiry);
            }
        }
    }
}

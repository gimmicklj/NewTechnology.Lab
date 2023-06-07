using System;

namespace Models
{
    /// <summary>
    /// 登录账户
    /// </summary>
    public class User
    {
        public long Id { get; set; }
        
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

    }

}

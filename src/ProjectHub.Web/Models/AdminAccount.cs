using System;

namespace ProjectHub.Web.Models
{
    /// <summary>
    /// 系统管理员账号。系统仅保留一个管理员账号。
    /// </summary>
    public class AdminAccount
    {
        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public int Iterations { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }
    }
}

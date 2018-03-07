using System;
using Microsoft.Extensions.Logging;
using ProjectHub.Web.Models;

namespace ProjectHub.Web.Services
{
    /// <summary>
    /// 管理员账号的读取、校验与登录记录。
    /// </summary>
    public class AdminAccountService
    {
        private const string DataFileName = "admin.json";
        private const string DefaultUserName = "admin";
        private const string DefaultPassword = "admin123";

        private static readonly object WriteSync = new object();

        private readonly IJsonDataStore _dataStore;
        private readonly ILogger<AdminAccountService> _logger;

        public AdminAccountService(IJsonDataStore dataStore, ILogger<AdminAccountService> logger)
        {
            _dataStore = dataStore;
            _logger = logger;
        }

        public bool ValidateCredentials(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var account = LoadOrCreate();
            if (!string.Equals(account.UserName, userName.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return PasswordHasher.Verify(password, account.PasswordSalt, account.PasswordHash, account.Iterations);
        }

        public void RecordLogin(string userName)
        {
            lock (WriteSync)
            {
                var account = LoadOrCreate();
                if (!string.Equals(account.UserName, userName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                account.LastLoginAt = DateTime.Now;
                _dataStore.Write(DataFileName, account);
            }
        }

        private AdminAccount LoadOrCreate()
        {
            lock (WriteSync)
            {
                var account = _dataStore.Read(DataFileName, CreateDefaultAccount);
                if (account == null)
                {
                    throw new InvalidOperationException("管理员账号数据不完整。");
                }

                return account;
            }
        }

        private AdminAccount CreateDefaultAccount()
        {
            var passwordHash = PasswordHasher.Create(DefaultPassword);

            _logger.LogWarning("未检测到管理员账号，已创建默认管理员账号 {UserName}。", DefaultUserName);

            return new AdminAccount
            {
                UserName = DefaultUserName,
                PasswordHash = passwordHash.Hash,
                PasswordSalt = passwordHash.Salt,
                Iterations = passwordHash.Iterations,
                CreatedAt = DateTime.Now
            };
        }
    }
}

using System;
using System.Security.Cryptography;

namespace ProjectHub.Web.Services
{
    /// <summary>
    /// 使用 PBKDF2-SHA256 对管理员密码进行加盐哈希。
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;

        // 旧账号仍按存储的迭代次数校验，并在下一次成功登录后自动升级。
        public const int DefaultIterations = 100000;

        public static PasswordHashResult Create(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("密码不能为空。", nameof(password));
            }

            var salt = new byte[SaltSize];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            var hash = DeriveKey(password, salt, DefaultIterations);

            return new PasswordHashResult
            {
                Hash = Convert.ToBase64String(hash),
                Salt = Convert.ToBase64String(salt),
                Iterations = DefaultIterations
            };
        }

        public static bool Verify(string password, string saltBase64, string hashBase64, int iterations)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(saltBase64) ||
                string.IsNullOrEmpty(hashBase64) ||
                iterations <= 0)
            {
                return false;
            }

            byte[] salt;
            byte[] expectedHash;

            try
            {
                salt = Convert.FromBase64String(saltBase64);
                expectedHash = Convert.FromBase64String(hashBase64);
            }
            catch (FormatException)
            {
                return false;
            }

            if (salt.Length == 0 || expectedHash.Length == 0)
            {
                return false;
            }

            var actualHash = DeriveKey(password, salt, iterations);
            return FixedTimeEquals(actualHash, expectedHash);
        }

        private static byte[] DeriveKey(string password, byte[] salt, int iterations)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return deriveBytes.GetBytes(HashSize);
            }
        }

        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
            {
                return false;
            }

            var difference = 0;
            for (var i = 0; i < left.Length; i++)
            {
                difference |= left[i] ^ right[i];
            }

            return difference == 0;
        }
    }

    public class PasswordHashResult
    {
        public string Hash { get; set; }

        public string Salt { get; set; }

        public int Iterations { get; set; }
    }
}

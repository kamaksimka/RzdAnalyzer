using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Helpers
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            string salt = GenerateSalt();

            using (SHA256 sha256 = SHA256.Create())
            {
                string passwordWithSalt = password + salt;
                byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordWithSalt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                StringBuilder hashStringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashStringBuilder.Append(b.ToString("x2"));
                }
                return $"{salt}:{hashStringBuilder.ToString()}";
            }
        }

        private static string GenerateSalt(int length = 16)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[length];
                rng.GetBytes(saltBytes);

                return Convert.ToBase64String(saltBytes);
            }
        }

        public static bool VerifyPassword(string storedHash,string inputPassword)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            string salt = parts[0];
            string storedHashValue = parts[1];

            string hashOfInput = HashPasswordWithSalt(inputPassword, salt);

            return hashOfInput == storedHashValue;
        }

        private static string HashPasswordWithSalt(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string passwordWithSalt = password + salt;

                byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordWithSalt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                StringBuilder hashStringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashStringBuilder.Append(b.ToString("x2"));
                }

                return hashStringBuilder.ToString();
            }
        }
    }
}

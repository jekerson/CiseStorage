using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Password
{
    public class PasswordService : IPasswordService
    {
        private const int SALT_LENGTH = 512;

        public string GenerateSalt()
        {
            byte[] salt = new byte[SALT_LENGTH];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        public string HashPassword(string plainTextPassword, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(plainTextPassword);

            byte[] hashBytes;
            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000, HashAlgorithmName.SHA512))
            {
                hashBytes = pbkdf2.GetBytes(SALT_LENGTH / 8);
            }

            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string plainTextPassword, string hashedPassword, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(plainTextPassword);

            byte[] hashBytes;
            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000, HashAlgorithmName.SHA512))
            {
                hashBytes = pbkdf2.GetBytes(SALT_LENGTH / 8);
            }

            string hashToCheck = Convert.ToBase64String(hashBytes);
            return hashToCheck == hashedPassword;
        }
    }

}

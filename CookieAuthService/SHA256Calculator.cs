using System.Security.Cryptography;
using System.Text;

namespace CookieAuthService
{
    /// <summary>
    /// Get sha256 value
    /// </summary>
    public static class Sha256Calculator
    {
        /// <summary>
        /// Get sha256 value
        /// </summary>
        /// <param name="plaintext">Plaintext</param>
        /// <returns>SHA256 value</returns>
        public static string GetSha256Value(string plaintext)
        {
            SHA256 sha256Calculator = SHA256.Create();
            byte[] hash = sha256Calculator.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}

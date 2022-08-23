using System.Security.Cryptography;
using System.Text;

#pragma warning disable SYSLIB0023
namespace Project.Utilities
{
    public class Tools
    {
        /// <summary>
        /// generate a random string with the given size using the input chars as alphabet.
        /// </summary>
        /// <param name="size">The size of the result string</param>
        /// <param name="chars">The alphabet used to generate the string (ex: ['0','1','2','3'] will generate a string of only those numbers)</param>
        /// <returns></returns>
        public static string GenerateRandomString(int size, char[] chars)
        {
            
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        public static string GenerateBase64Code()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}

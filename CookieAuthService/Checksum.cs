using System.Security.Cryptography;
using System.Text;
using System.Timers;

namespace CookieAuthService
{
    public static class Checksum
    {
        static string nowChecksum;
        static string lastChecksum;
        public static string checksum => nowChecksum;
        static System.Timers.Timer? updateTimer;

        /// <summary>
        /// Initialize checksum calculator
        /// </summary>
        /// <param name="updateTime"></param>
        public static void Initialize(int updateTime)
        {
            //Set checksome at first time
            UpdateChecksum();
            lastChecksum = nowChecksum;

            //Set auto update
            updateTimer = new System.Timers.Timer(updateTime);
        }

        /// <summary>
        /// Timer event to update checksum
        /// </summary>
        static void UpdateChecksumEvent(Object source, ElapsedEventArgs e)
        {
            UpdateChecksum();
        }

        /// <summary>
        /// Generate new checksum
        /// </summary>
        static void UpdateChecksum()
        {
            lastChecksum = nowChecksum;

            SHA256 newChecksumCalculator = SHA256.Create();
            byte[] hash = newChecksumCalculator.ComputeHash(Encoding.UTF8.GetBytes($"Updated{DateTime.Now.Ticks}checksum{Guid.NewGuid()}with{Random.Shared.Next()}random{Random.Shared.Next()}value{Random.Shared.Next()}"));   //Generate checksum with compax string
            StringBuilder newChecksumBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                newChecksumBuilder.Append(hash[i].ToString("x2"));
            }
            nowChecksum = newChecksumBuilder.ToString();
        }

        public static bool Check(string target)
        {
            return target == nowChecksum || target == lastChecksum;
        }
    }
}

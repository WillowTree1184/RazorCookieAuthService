namespace CookieAuthService
{
    /// <summary>
    /// A service to manage the checksum
    /// </summary>
    public static class ChecksumService
    {
        static string nowChecksum;
        static string lastChecksum;
        public static string checksum => nowChecksum;

        static System.Timers.Timer? updateTimer;
        /// <summary>
        /// Occurs when the checksum updated
        /// </summary>
        public static event EventHandler<string>? ChecksumUpdated;

        /// <summary>
        /// Initialize the checksum calculator
        /// </summary>
        /// <param name="updateInterval">Checksum update interval</param>
        public static void Initialize(int updateInterval)
        {
            UpdateChecksum();
            lastChecksum = nowChecksum;     //It is the first time to generate the checksum, so the lastChecksum must equals the nowChecksum

            updateTimer = new System.Timers.Timer(updateInterval);
            updateTimer.Elapsed += (source, e) => UpdateChecksum();
            updateTimer.AutoReset = true;
            updateTimer.Enabled = true;
        }

        /// <summary>
        /// Generate a new checksum
        /// </summary>
        static void UpdateChecksum()
        {
            lastChecksum = nowChecksum;
            nowChecksum = Sha256Calculator.GetSha256Value($"{Guid.NewGuid()}Checksum{Random.Shared.Next()}{Random.Shared.Next()}{Random.Shared.Next()}{Random.Shared.Next()}{Random.Shared.Next()}{Random.Shared.Next()}{DateTime.Now.Ticks}");   //Use complex and unique strings to generate checksums
            ChecksumUpdated?.Invoke(lastChecksum, nowChecksum);
        }

        /// <summary>
        /// Check the checksum
        /// </summary>
        /// <param name="checksum">The checksum</param>
        /// <returns>When passed, return true; Otherwise, it returns false</returns>
        public static bool Check(string checksum) => checksum == nowChecksum || checksum == lastChecksum;
    }
}

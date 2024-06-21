namespace CookieAuthService
{
    public interface IAuthService
    {
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="token">The token that will be written to the cookie</param>
        Task LoginAsync(string token);

        /// <summary>
        /// Get token
        /// </summary>
        /// <returns>The token</returns>
        Task<string> GetTokenAsync();

        /// <summary>
        /// Logout
        /// </summary>
        Task LogoutAsync();
    }
}

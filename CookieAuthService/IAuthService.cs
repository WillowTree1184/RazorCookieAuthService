namespace CookieAuthService
{
    /// <summary>
    /// A service to login, logout and get the token which have been wrote in cookie
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="token">The token that will be written to the cookie</param>
        void Login(string token);

        /// <summary>
        /// Get token
        /// </summary>
        /// <returns>The token</returns>
        Task<string?> GetTokenAsync();

        /// <summary>
        /// Logout
        /// </summary>
        void Logout();
    }
}

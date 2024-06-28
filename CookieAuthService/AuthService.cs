using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CookieAuthService
{
    /// <summary>
    /// A service to login, logout and get the token which have been wrote in cookie
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly AuthController authController;
        private readonly NavigationManager navigationManager;
        public readonly IHttpContextAccessor httpContextAccessor;
        public readonly AuthenticationStateProvider authenticationStateProvider;

        public AuthService(AuthController authController, NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor, AuthenticationStateProvider authenticationStateProvider)
        {
            this.authController = authController;
            this.navigationManager = navigationManager;
            this.httpContextAccessor = httpContextAccessor;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="token">The token that will be written to the cookie</param>
        public void Login(string token)
        {
            navigationManager.NavigateTo($"/api/auth/login?tokenId={TokenDictionaryService.Regist(token)}&checksum={ChecksumService.checksum}", true, true);
        }

        /// <summary>
        /// Get token
        /// </summary>
        /// <returns>The token</returns>
        public async Task<string?> GetTokenAsync() => (await authenticationStateProvider.GetAuthenticationStateAsync()).User.FindFirst(c => c.Type == "token")?.Value;

        /// <summary>
        /// Logout
        /// </summary>
        public void Logout()
        {
            navigationManager.NavigateTo($"/api/auth/logout", true, true);
        }
    }
}

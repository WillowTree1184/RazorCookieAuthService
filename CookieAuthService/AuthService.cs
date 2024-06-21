using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json;

namespace PkusForum.CookieAuthService
{
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
        public async Task LoginAsync(string token)
        {
            navigationManager.NavigateTo($"/api/auth/login?token={token}&checksum={Checksum.checksum}", true, true);
        }

        /// <summary>
        /// Get token
        /// </summary>
        /// <returns>The token</returns>
        public async Task<string> GetTokenAsync()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User.FindFirst(c => c.Type == "token").Value;
        }

        /// <summary>
        /// Logout
        /// </summary>
        public async Task LogoutAsync()
        {
            navigationManager.NavigateTo($"/api/auth/logout", true, true);
        }
    }
}

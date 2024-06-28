using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CookieAuthService
{
    /// <summary>
    /// A controller to write and delete cookie
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IHttpContextAccessor httpContextAccessor;
        static string indexPageURL;
        static string loginPageURL;
        static KeyValuePair<string, string> queryWhenTimeOut;
        static KeyValuePair<string, string> queryWhenTokenIdError;

        /// <summary>
        /// Initialize AuthController
        /// </summary>
        /// <param name="indexPageURL">The url which will be redirect when login successful</param>
        /// <param name="loginPageURL">The url which will be redirect when login unsuccessful or logout successful</param>
        /// <param name="queryWhenTimeOut">The query which will been added when login time out</param>
        /// <param name="queryWhenTokenIdError">The query which will been added if the tokenId is unregisted when login</param>
        public static void Initialize(string indexPageURL, string loginPageURL, KeyValuePair<string, string> queryWhenTimeOut, KeyValuePair<string, string> queryWhenTokenIdError)
        {
            AuthController.indexPageURL = indexPageURL;
            AuthController.loginPageURL = loginPageURL;
            AuthController.queryWhenTimeOut = queryWhenTimeOut;
            AuthController.queryWhenTokenIdError = queryWhenTokenIdError;
        }

        public AuthController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Check the checksum and write the token to the cookie.
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="checksum">The checksum</param>
        /// <returns>
        /// When check success：Redirect to indexPageURL
        /// When check fail：Redirect to loginPageURL
        /// </returns>
        [Route("login")]
        [HttpGet]
        public async Task<ActionResult>? Login(string tokenId, string checksum)
        {
            if (!ChecksumService.Check(checksum))
            {
                return Redirect($"{loginPageURL}?{queryWhenTimeOut.Key}={queryWhenTimeOut.Value}");
            }

            string token;
            if (!TokenDictionaryService.TryPopToken(tokenId, out token))
            {
                return Redirect($"{loginPageURL}?{queryWhenTokenIdError.Key}={queryWhenTokenIdError.Value}");
            }
            Console.WriteLine(token);
            
            List<Claim> claims = new List<Claim>
            {
                new Claim("token", token)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMonths(1)
            };

            await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return Redirect(indexPageURL);
        }

        /// <summary>
        /// Delete cookie
        /// </summary>
        /// <returns>Redirect to loginPageURL</returns>
        [Route("logout")]
        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect(loginPageURL);
        }
    }
}

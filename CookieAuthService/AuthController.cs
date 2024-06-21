﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PkusForum.CookieAuthService
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IHttpContextAccessor httpContextAccessor;
        public static string indexPageURL = "/";
        public static string loginPageURL = "/login";

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
        public async Task<ActionResult> Login(string token, string checksum)
        {
            //Check checksum
            if (!Checksum.Check(checksum))
            {
                return Redirect($"{loginPageURL}?error=请求超时");
            }

            //Write cookie
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
        /// 删除授权cookie
        /// </summary>
        /// <returns>跳转至loginPageURL</returns>
        [Route("logout")]
        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect(loginPageURL);
        }
    }
}
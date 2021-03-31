using System;
using Microsoft.AspNetCore.Http;

namespace CookieServer
{
    public static class CookieUpdater
    {
        public static void ClearCookie(HttpContext httpContext, string cookieName)
        {
            httpContext.Response.Cookies.Delete($"{cookieName}");
        }

        public static void WriteCookie(HttpContext httpContext, string cookieName, string value, TimeSpan? duration = null)
        {
            httpContext.Response.Cookies.Append(
                cookieName, value
                , new CookieOptions
                {
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.Add(duration ?? TimeSpan.FromMinutes(5)),
                    HttpOnly = true
                });
        }
    }
}
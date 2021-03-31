using System;
using System.Text;
using System.Threading.Tasks;
using CookieServer;
using Microsoft.AspNetCore.Http;

namespace Nrk.Innlogging.LoginPod.LoginMiddleware
{
    public class CookieExposerMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieExposerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
           
            if (context.Request.Path.ToString().StartsWith("/cookies/see"))
            {
                var sb = new StringBuilder();

                foreach (var cookie in context.Request.Cookies)
                {
                    sb.AppendLine($"{cookie.Key} => {cookie.Value}");
                }
                context.Response.ContentType = "text/plain";

                await context.Response.WriteAsync(sb.ToString());
                return;
            }
            if (context.Request.Path.ToString().StartsWith("/cookies/clear"))
            {
                foreach (var cookie in context.Request.Cookies)
                {
                    context.Response.Cookies.Delete(cookie.Key);
                }

                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("all cookies removed");
                return;
            }
            if (context.Request.Path.ToString().StartsWith("/cookies/write/"))
            {
                var p = context.Request.Path.ToString().Replace("/cookies/write/", "").Split("/");
                CookieUpdater.WriteCookie(context, p[0], p[1], TimeSpan.FromMinutes(5));
                context.Response.Redirect("/cookies/see");
                return;
            }
            if (context.Request.Path.ToString().StartsWith("/cookies/remove/"))
            {
                var p = context.Request.Path.ToString().Replace("/cookies/remove/", "");
                CookieUpdater.ClearCookie(context, p);
                context.Response.Redirect("/cookies/see");
                return;
            }
            if (context.Request.Path.ToString().StartsWith("/cookies"))
            {
                context.Response.ContentType = "text/plain";

                await context.Response.WriteAsync(@"Usage: 

To see all cookies received on request by server: 
/cookies/see

To make server write a new cookie (which will last for 5 minutes)
/cookies/write/<cookiename>/<cookievalue>

To make server clear a cookie
/cookies/remove/<cookiename>

To make server clear all cookies
/cookies/clear


----
To reproduce problem:
A: (using only one browser (e.g. edge)
- go to http://localhost:8080/cookies/write/a/someStringHere
- you will be sent to http://localhost:8080/cookies/see which will display which cookies the server is receiving on the request
- see that the cookie is available in the client using edge dev tools (or corresponding)
- delete the cookie from the client using edge dev tools (or corresponding)
- refresh http://localhost:8080/cookies/see and see that the server is still receiving the (now deleted cookie)

B: (using two browsers)
- go to http://localhost:8080/cookies/write/b/someOtherStringHere in browser A (e.g. edge)
- you will be sent to http://localhost:8080/cookies/see which will display which cookies the server is receiving on the request
- navigate to http://localhost:8080/cookies/see with a different browser (e.g. firefox)
- observe that cookie 'b' is not present on the client, but is still being sent to the server

To verify that the problem is connected to the ingress:
do the exact same tests as before, but instead of going through the ingress, go directly to the service (track down the port number through the tye console)

");
                return;
            }

            await _next(context);
        }

    }
}
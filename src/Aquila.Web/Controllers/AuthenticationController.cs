using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Aquila.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet("~/login")]
        public ActionResult LogIn(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("~/logout"), HttpPost("~/logout")]
        public ActionResult LogOut()
        {
            // is redirected from the identity provider after a successful authorization flow and
            // to redirect the user agent to the identity provider to sign out.
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
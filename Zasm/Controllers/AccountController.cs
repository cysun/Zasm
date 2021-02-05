using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Zasm.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("oidc", new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

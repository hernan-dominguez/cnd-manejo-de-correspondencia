using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;

namespace CWA.Web.Pages
{
    [AllowAnonymous]
    public class SignOutModel : PageModel
    {
        public readonly SignInManager<AppUser> _sim;
        public readonly LinkGenerator _links;

        public SignOutModel(SignInManager<AppUser> SignInManagerService, LinkGenerator Links)
        {
            _sim = SignInManagerService;
            _links = Links;
        }

        public async Task<IActionResult> OnPost()
        {
            if (_sim.IsSignedIn(User))
            {
                await _sim.SignOutAsync();

                if (User.HasClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod","Microsoft"))
                {
                    // Sign out of MS account
                    var redirectUri = _links.GetUriByPage(Request.HttpContext, "/Index");

                    return new RedirectResult($"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={redirectUri}");
                }                
            }            

            return new RedirectResult("/Index");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CWA.Web.Pages
{
    [AllowAnonymous]
    public class ArkModel : PageModel
    {
        private readonly SignInManager<AppUser> _sim;

        public string ReturnUrl { get; set; }

        public ArkModel(SignInManager<AppUser> SimService) => _sim = SimService;

        public IActionResult OnGet() => RedirectToPage("/SignIn");

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("/Ark", pageHandler: "Callback", values: new { returnUrl });
            var properties = _sim.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                //ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("/SignIn", new { ReturnUrl = returnUrl });
            }
            
            var info = await _sim.GetExternalLoginInfoAsync();

            if (info == null)
            {
                //ErrorMessage = "Error loading external login information.";
                return RedirectToPage("/SignIn", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _sim.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            
            if (result.Succeeded) return LocalRedirect(returnUrl);

            return RedirectToPage("/SignIn", new { ReturnUrl = returnUrl });
        }
    }
}

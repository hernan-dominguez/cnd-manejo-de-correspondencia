using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.Entities.Identity;
using CWA.Shared.Helpers;
using CWA.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CWA.Models.Comun;
using Microsoft.EntityFrameworkCore;

namespace CWA.Web.Pages
{
    [AllowAnonymous]
    public class SignInModel : PageModel
    {
        private readonly SignInManager<AppUser> _sim;

        [BindProperty]
        public LoginCredentials ModelData { get; set; }

        public string HeadMessage { get; set; } = "Por favor inicie sesión para continuar";
        public string ReturnUrl { get; set; }
        public bool IsError = false;

        public SignInModel(SignInManager<AppUser> SignInManagerService)
        {
            _sim = SignInManagerService;
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            if (_sim.IsSignedIn(User))
            {
                return new RedirectResult("/Index");
            }

            returnUrl ??= Url.Content("~/");

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Check lock status
                var lookup = await _sim.UserManager.Users.Where(w => w.NormalizedUserName == ModelData.UserName.ToUpper()).FirstOrDefaultAsync();

                if (lookup is null)
                {
                    HeadMessage = "Usuario y/o contraseña incorrectos";
                    IsError = true;
                    return Page();
                }
                else
                {
                    if (lookup.Locked)
                    {
                        HeadMessage = "Usuario y/o contraseña incorrectos";
                        IsError = true;
                        return Page();
                    }
                }


                //crear usuario runone egarcia




                // Attempt signin
                var result = await _sim.PasswordSignInAsync(ModelData.UserName, ModelData.PassWord, ModelData.KeepSignedIn, false);

                
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    HeadMessage = "Usuario y/o contraseña incorrectos";
                    IsError = true;
                }                
            }
            else
            {
                HeadMessage = "Usuario y/o contraseña incorrectos";
                IsError = true;
            }

            return Page();
        }
    }
}

using CWA.AccessControl.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CWA.Web.Pages.GrandesClientes
{
    [Authorize(Policy = PolicyNames.AdminSectionPolicy)]
    public class MedicionSmecModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

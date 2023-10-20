using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CWA.Web.Pages.Admin
{
    public class CatalogoModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string CatalogoId { get; set; }

        public void OnGet()
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CWA.Web.Pages.Admin
{
    public class AgenteModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string AgenteId { get; set; }

        public void OnGet()
        {
        }
    }
}

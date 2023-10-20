using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.GrandesClientes;
using CWA.Models.GrandesClientes.View;
using CWA.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CWA.Web.Pages.GrandesClientes
{
    [Authorize(Policy = PolicyNames.GCMedidoresDistPolicy)]
    public class MedidoresDistModel : PageModel
    {
        // Services
        private readonly GCService _gcdata;

        // Models
        public List<GCMedidoresDistView> ViewModel { get; set; }

        // Routing
        [BindProperty(SupportsGet = true)]
        public int RegistroId { get; set; }

        public ViewPack<object> RegistroContext;

        // Presentation
        public string Nombre = "";

        public MedidoresDistModel(GCService DataService) => _gcdata = DataService;

        public async Task<IActionResult> OnGetAsync()
        {
            RegistroContext = await _gcdata.ViewRegistroContextAsync(RegistroId);
            RegistroContext.Add("current", "medidores");

            ViewModel = await _gcdata.ViewMedidoresDistAsync(RegistroId);

            Nombre = $"{RegistroContext["nombre"]}";

            return Page();
        }
    }
}

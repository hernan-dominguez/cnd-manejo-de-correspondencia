using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services;
using CWA.Application.Services.GrandesClientes;
using CWA.Entities.GrandesClientes;
using CWA.Models.Comun;
using CWA.Models.GrandesClientes.View;
using CWA.Shared.Helpers;
using CWA.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace CWA.Web.Pages.GrandesClientes
{
    [Authorize(Policy = PolicyNames.GCMedicionesPolicy)]
    public class MedicionesModel : PageModel
    {
        // Services
        private readonly GCService _gcdata;
        private readonly GCAccessControlService _gccontrol;

        // Models
        public List<GCMedicionesView> ViewModel { get; set; }

        // Routing
        [BindProperty(SupportsGet = true)]
        public int RegistroId { get; set; }

        // Presentation
        public bool ShowUpdate = false;
        public string Nombre = "";

        public ViewPack<object> RegistroContext;

        public MedicionesModel(GCService DataService, GCAccessControlService ControlService)
        {
            _gcdata = DataService;
            _gccontrol = ControlService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get related info
            RegistroContext = await _gcdata.ViewRegistroContextAsync(RegistroId);
            RegistroContext.Add("current", "mediciones");

            ViewModel = await _gcdata.ViewMedicionesAsync(RegistroId);

            Nombre = $"{RegistroContext["nombre"]}";
            ShowUpdate = await _gccontrol.AllowMedicionPostAsync(RegistroId);

            return Page();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl.Services;
using CWA.Application.Services.Protecciones;
using CWA.Models.Protecciones.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CWA.Web.Pages.Protecciones
{
    public class IndexModel : PageModel
    {
        // Services
        private readonly PROTService _protservice;
        private readonly PROTAccessControlService _protcontrol;

        // Models
        public List<PROTRegistroView> ViewModel { get; set; }

        // Presentation
        public bool CanCreate = false;
        public bool CanView = false;

        public IndexModel(PROTService PROTView, PROTAccessControlService PROTControl)
        {
            _protservice = PROTView;
            _protcontrol = PROTControl;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CanCreate = await _protcontrol.AllowRegistrarGetPostAsync();
            CanView = await _protcontrol.AllowRegistroGetAsync();
            ViewModel = await _protservice.ViewRegistrosAsync();

            return Page();
        }
    }
}

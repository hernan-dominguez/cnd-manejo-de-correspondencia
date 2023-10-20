using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.GrandesClientes;
using CWA.Models.GrandesClientes.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CWA.Web.Pages.GrandesClientes
{    
    public class IndexModel : PageModel
    {
        // Services
        private readonly GCService _gcdata;
        private readonly GCAccessControlService _gccontrol;
        private readonly AppAccessControlService _access;

        // Models
        public List<GCRegistroView> ViewModel { get; set; }

        // Presentation
        public bool CanCreate = false;
        public bool CanView = false;
        public bool ShowMore = false;

        public IndexModel(GCService DataService, GCAccessControlService ControlService, AppAccessControlService Access)
        {
            _gcdata = DataService;
            _gccontrol = ControlService;
            _access = Access;
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            CanCreate = await _gccontrol.AllowRegistrarGetPostAsync();
            CanView = await _gccontrol.AllowRegistroGetAsync();
            ViewModel = await _gcdata.ViewRegistrosAsync();
            
            ShowMore = await _access.IsInGroup(new[] { GroupNames.CndGroup, GroupNames.AdminGroup });

            return Page();
        }        
    }
}

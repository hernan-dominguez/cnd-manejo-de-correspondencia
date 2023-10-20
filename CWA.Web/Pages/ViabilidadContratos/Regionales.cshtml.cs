using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.ViabilidadContratos;
using CWA.Entities.ViabilidadContratos;
using CWA.Models.ViabilidadContratos.View;
using CWA.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CWA.Web.Pages.ViabilidadContratos
{
    [Authorize(Policy = PolicyNames.VCRegionalesPolicy)]
    public class RegionalesModel : PageModel
    {
        // Services
        private readonly VCService _vcdata;
        private readonly AppAccessControlService _access;

        // Models
        public List<VCRegionalView> ViewModel { get; set; }

        // Presentation
        public bool CanCreate;
        public bool CanView;
        public bool ShowMore;

        public ViewPack<object> RegistroContext;

        public RegionalesModel(VCService DataService, AppAccessControlService Access)
        {
            _vcdata = DataService;
            _access = Access;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            RegistroContext = await _vcdata.ViewRegistroContext<VCRegional>();
            RegistroContext.Add("current", "regionales");

            CanCreate = await _access.HasClaimAsync(ClaimNames.VCUpdate);
            CanView = await _access.HasClaimAsync(ClaimNames.VCView);
            ShowMore = await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup });

            ViewModel = !ShowMore ? await _vcdata.ViewRegionalesAsync((int)RegistroContext["agt-id"]) : await _vcdata.ViewRegionalesAsync();

            return Page();
        }
    }
}

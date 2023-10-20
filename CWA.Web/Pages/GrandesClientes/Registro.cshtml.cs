using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl.Authorization.GrandesClientes;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.GrandesClientes;
using CWA.Entities.GrandesClientes;
using CWA.Models.GrandesClientes.View;
using CWA.Shared.Extensions;
using CWA.Shared.Helpers;
using CWA.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace CWA.Web.Pages.GrandesClientes
{
    [Authorize(Policy = PolicyNames.GCRegistroPolicy)]
    public class RegistroModel : PageModel
    {
        // Services
        private readonly GCService _gcdata;
        private readonly GCAccessControlService _gccontrol;
        private readonly IOptions<ApplicationSettings> _options;

        // Models
        [Required]
        [DataType(DataType.Date)]
        [BindProperty]        
        [Display(Name = "Fecha de Entrada")]
        public DateTime? FechaEntrada { get; set; }

        public GCRegistroView ViewModel { get; set; }

        // Routing
        [BindProperty(SupportsGet = true)]
        public int RegistroId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Handler { get; set; }

        // Presentation
        public bool ShowUpdate = false;
        public bool ShowNotify = false;
        public bool ShowAgente = false;
        public string Nombre = "";

        public ViewPack<object> RegistroContext;

        public RegistroModel(GCService DataService, GCAccessControlService ControlService, IOptions<ApplicationSettings> Options)
        {
            _gcdata = DataService;
            _gccontrol = ControlService;
            _options = Options;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get related info
            RegistroContext = await _gcdata.ViewRegistroContextAsync(RegistroId);
            RegistroContext.Add("current", "registro");

            ViewModel = await _gcdata.ViewRegistroAsync(RegistroId);
            ShowAgente = ViewModel.Tipo == "PASIVO";
            
            ShowUpdate = await _gccontrol.AllowRegistroPostAsync(RegistroId, GCConstants.UPDATE);
            ShowNotify = await _gccontrol.AllowRegistroPostAsync(RegistroId, GCConstants.NOTIFY);

            Nombre = ViewModel.Nombre;
            FechaEntrada = ViewModel.FechaEntrada;            

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            PageOperationResult result = new();

            if (!ModelState.IsValid)
            {
                result.Message = _options.Value.GrandesClientes.Strings.FechaRequerido;
                result.Success = false;
                return new JsonResult(result);
            }

            if (await _gcdata.InvalidFechaEntradaAsync(RegistroId, FechaEntrada.Value))
            {
                result.Message = _options.Value.GrandesClientes.Strings.FechaInvalida;
                result.Success = false;
                return new JsonResult(result);
            }

            result = await _gcdata.SaveFechaEntradaAsync(RegistroId, FechaEntrada.Value, "GCN02");

            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ApplyFormat(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostNotifyAsync()
        {
            PageOperationResult result = await _gcdata.NotifyAuthorizationAsync(RegistroId);
            
            return new JsonResult(result);
        }
    }
}

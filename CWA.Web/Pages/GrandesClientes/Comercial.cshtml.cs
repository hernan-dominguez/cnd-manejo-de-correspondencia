using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Comun;
using CWA.Application.Services.GrandesClientes;
using CWA.Entities.GrandesClientes;
using CWA.Models.GrandesClientes.Edit;
using CWA.Models.GrandesClientes.View;
using CWA.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using CWA.Shared.Helpers;
using CWA.Shared.Extensions;
using CWA.Models.GrandesClientes.Validation;
using CWA.Web.Extensions;
using CWA.AccessControl.Authorization.GrandesClientes;

namespace CWA.Web.Pages.GrandesClientes
{
    [Authorize(Policy = PolicyNames.GCComercialPolicy)]
    public class ComercialModel : PageModel
    {
        // Services
        private readonly GCService _gcdata;
        private readonly GCAccessControlService _gccontrol;
        private readonly AppAccessControlService _access;
        private readonly IOptions<ApplicationSettings> _options;

        // Models
        [BindProperty]
        public GCComercialEdit EditModel { get; set; }

        public GCComercialView ViewModel { get; set; }

        // Lists
        public List<SelectListItem> Provincias = new();

        // Routing
        [BindProperty(SupportsGet = true)]
        public int RegistroId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int ComercialId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Handler { get; set; }

        // Presentation
        public bool ShowView = false;
        public bool ShowUpdate = false;
        public bool ShowApprove = false;
        public string Nombre = "";
        public DateTime LastSave;

        public ViewPack<object> RegistroContext;

        public ComercialModel(GCService DataService, GCAccessControlService ControlService, AppAccessControlService Access, IOptions<ApplicationSettings> Options)
        {
            _gcdata = DataService;
            _gccontrol = ControlService;
            _access = Access;
            _options = Options;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            RegistroContext = await _gcdata.ViewRegistroContextAsync<int, GCComercial>(RegistroId, ComercialId);
            RegistroContext.Add("current", "comercial");

            Nombre = $"{RegistroContext["nombre"]}";
            LastSave = (DateTime)RegistroContext["modfecha"];

            ShowUpdate = await _gccontrol.AllowComercialPostAsync(RegistroId, ComercialId, GCConstants.UPDATE);
            ShowApprove = await _gccontrol.AllowComercialPostAsync(RegistroId, ComercialId, GCConstants.APPROVE);

            if (ShowUpdate || ShowApprove)
            {
                ShowUpdate = true;
                EditModel = await _gcdata.GetDatosAsync<int, GCComercial, GCComercialEdit>(ComercialId);
            }
            else
            {
                ShowView = true;
                ViewModel = await _gcdata.ViewDatosAsync<int, GCComercial, GCComercialView>(ComercialId);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            PageOperationResult result = new();
            var comercial = await _gcdata.GetDatosAsync<int, GCComercial, GCComercialEdit>(ComercialId);
            bool NotifyOwner = false;

            if (await _access.HasClaimAsync(ClaimNames.GCApprove))
            {
                NotifyOwner = true;

                if (!ModelState.IsValid)
                {
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Success = false;
                    result.Content = ModelState.GetErrorMessages();

                    return new JsonResult(result);
                }

                if (!EditModel.MontoGarantiaNacional.HasValue && !EditModel.MontoGarantiaRegional.HasValue)
                {
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Success = false;
                    result.Content = new object[]
                    {
                        new { Key = "EditModel-MontoGarantiaNacional", Message = $"Al menos se debe completar este campo." },
                        new { Key = "EditModel-MontoGarantiaRegional", Message = $"Al menos se debe completar este campo." }
                    };

                    return new JsonResult(result);
                }

                EditModel.CuentaBancaria = comercial.CuentaBancaria;
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Success = false;
                    result.Content = ModelState.GetErrorMessages();

                    return new JsonResult(result);
                }

                if (EditModel.CuentaBancaria.Empty())
                {
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Success = false;
                    result.Content = new { Key = "EditModel-CuentaBancaria", Message = "The field Cuenta Bancaria is required." };
                    
                    return new JsonResult(result);
                }

                EditModel.MontoGarantiaNacional = comercial.MontoGarantiaNacional;
                EditModel.MontoGarantiaRegional = comercial.MontoGarantiaRegional;
            }

            result = await _gcdata.SaveDatosAsync<int, GCComercialEdit, GCComercial>(ComercialId, EditModel, "GCN05", RegistroId, NotifyOwner);
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ApplyFormat(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            PageOperationResult result = new();

            // Validate data
            if (await _gcdata.InvalidItemDataAsync<int, GCComercial, GCComercialValidation>(ComercialId))
            {
                result.Message = _options.Value.GrandesClientes.Strings.NoAprobar;
                result.Success = false;
                return new JsonResult(result);
            }

            result = await _gcdata.ApproveAsync<int, GCComercial>(ComercialId, RegistroId, "GCN05");
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ApplyFormat(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }
    }
}

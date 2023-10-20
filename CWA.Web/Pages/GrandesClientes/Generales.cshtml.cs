using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl;
using CWA.AccessControl.Authorization.GrandesClientes;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services;
using CWA.Application.Services.Comun;
using CWA.Application.Services.GrandesClientes;
using CWA.Entities.GrandesClientes;
using CWA.Models.Comun;
using CWA.Models.GrandesClientes.Edit;
using CWA.Models.GrandesClientes.View;
using CWA.Shared.Extensions;
using CWA.Shared.Helpers;
using CWA.Web.Extensions;
using CWA.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace CWA.Web.Pages.GrandesClientes
{
    [Authorize(Policy = PolicyNames.GCGeneralesPolicy)]
    public class GeneralesModel : PageModel
    {
        // Services
        private readonly GCService _gcdata;
        private readonly GCAccessControlService _gccontrol;
        private readonly CatalogoService _catalogo;
        private readonly IOptions<ApplicationSettings> _options;

        // Models
        [BindProperty]
        public GCGeneralesEdit EditModel { get; set; }

        public GCGeneralesView ViewModel { get; set; }

        // Lists
        public List<SelectListItem> Provincias = new();

        // Routing
        [BindProperty(SupportsGet = true)]
        public int RegistroId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int GeneralesId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Handler { get; set; }

        // Presentation
        public bool ShowView = false;
        public bool ShowUpdate = false;
        public bool ShowApprove = false;
        public string Nombre = "";
        public DateTime LastSave;

        public ViewPack<object> RegistroContext;

        public GeneralesModel(GCService DataService, GCAccessControlService ControlService, CatalogoService Catalogo, IOptions<ApplicationSettings> Options)
        {
            _gcdata = DataService;
            _gccontrol = ControlService;
            _catalogo = Catalogo;
            _options = Options;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get related info
            RegistroContext = await _gcdata.ViewRegistroContextAsync<int, GCGenerales>(RegistroId, GeneralesId);
            RegistroContext.Add("current", "generales");

            Nombre = $"{RegistroContext["nombre"]}";
            LastSave = (DateTime)RegistroContext["modfecha"];

            // Check if the item is updatable
            ShowUpdate = await _gccontrol.AllowGeneralesPostAsync(RegistroId, GeneralesId, GCConstants.UPDATE);

            if (ShowUpdate)
            {
                EditModel = await _gcdata.GetDatosAsync<int, GCGenerales, GCGeneralesEdit>(GeneralesId);
                Provincias = await _catalogo.GetForSelectByGrupoAsync("PROVINCIA");
            }
            else
            {
                ShowView = true;
                ViewModel = await _gcdata.ViewDatosAsync<int, GCGenerales, GCGeneralesView>(GeneralesId);
            }

            // Check if item can be approved
            ShowApprove = await _gccontrol.AllowGeneralesPostAsync(RegistroId, GeneralesId, GCConstants.APPROVE);

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            PageOperationResult result = new();

            EditModel.WebUrl = !EditModel.WebUrl.Empty() ? $"http://{EditModel.WebUrl}".ToLower() : EditModel.WebUrl;

            ModelState.ClearValidationState(nameof(EditModel));

            if (!TryValidateModel(EditModel, nameof(EditModel)))
            {
                result.Message = _options.Value.Strings.DatosInvalidos;
                result.Success = false;
                result.Content = ModelState.GetErrorMessages();

                return new JsonResult(result);
            }

            result = await _gcdata.SaveDatosAsync<int, GCGeneralesEdit, GCGenerales>(GeneralesId, EditModel, "GCN04", RegistroId);
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ApplyFormat(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            PageOperationResult result = new();

            // Validate data
            if (await _gcdata.InvalidItemDataAsync<int, GCGenerales, GCGeneralesEdit>(GeneralesId))
            {
                result.Message = _options.Value.GrandesClientes.Strings.NoAprobar;
                result.Success = false;
                return new JsonResult(result);
            }

            result = await _gcdata.ApproveAsync<int, GCGenerales>(GeneralesId, RegistroId, "GCN04");
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ApplyFormat(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Comun;
using CWA.Application.Services.Protecciones;
using CWA.Entities.Identity;
using CWA.Entities.Protecciones;
using CWA.Models.Protecciones.Edit;
using CWA.Models.Protecciones.View;
using CWA.Shared.Helpers;
using CWA.Web.Extensions;
using CWA.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace CWA.Web.Pages.Protecciones
{
    [Authorize(Policy = PolicyNames.PROTGeneralesPolicy)]
    public class GeneralesModel : PageModel
    {
        // Services
        private readonly PROTService _protservice;
        private readonly PROTAccessControlService _protcontrol;
        private readonly CatalogoService _catalogo;
        private readonly IOptions<ApplicationSettings> _options;
        private readonly UserManager<AppUser> _userMgr;

        // Models
        [BindProperty]
        public PROTGeneralesEdit EditModel { get; set; }

        public PROTGeneralesView ViewModel { get; set; }

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
        public string RazonSocial = "";
        public DateTime LastSave;
        public DateTime? Approved;

        // Options
        readonly string DocumentsRoot;
        readonly string DocumentsTemp;

        public ViewPack<object> InformationContext;

        public GeneralesModel(PROTService DataService, PROTAccessControlService ControlService, CatalogoService Catalogo,
            IOptions<ApplicationSettings> Options, UserManager<AppUser> userMgr)
        {
            _protservice = DataService;
            _protcontrol = ControlService;
            _catalogo = Catalogo;
            _options = Options;
            _userMgr = userMgr;
            DocumentsRoot = _options.Value.Protecciones.DocumentsRoot;
            DocumentsTemp = _options.Value.Protecciones.DocumentsTemp;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get related info
            InformationContext = await _protservice.ViewInfoAsync<int, PROTGenerales>(RegistroId, GeneralesId);

            RazonSocial = $"{InformationContext["razonsocial"]}";
            LastSave = (DateTime)InformationContext["modfecha"];
            Approved = InformationContext["fechaaprobacion"] as DateTime?;

            // Check if the item is updatable
            ShowUpdate = await _protcontrol.AllowGeneralesPostAsync(RegistroId, GeneralesId, PROTConstants.UPDATE);

            if (ShowUpdate)
            {
                EditModel = await _protservice.GetDatosAsync<int, PROTGenerales, PROTGeneralesEdit>(GeneralesId);
                Provincias = await _catalogo.GetForSelectByGrupoAsync("PROVINCIA");
            }
            else
            {
                ShowView = true;
                ViewModel = await _protservice.ViewDatosAsync<int, PROTGenerales, PROTGeneralesView>(GeneralesId);
            }

            // Check if item can be approved
            ShowApprove = await _protcontrol.AllowGeneralesPostAsync(RegistroId, GeneralesId, PROTConstants.APPROVE);

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            PageOperationResult result = new();

            if (!ModelState.IsValid)
            {
                result.Message = _options.Value.Strings.DatosInvalidos;
                result.Success = false;
                result.Content = ModelState.GetErrorMessages();

                return new JsonResult(result);
            }

            var registro = await _protservice.ViewRegistroAsync(RegistroId);

            result = await _protservice.SaveDatosAsync<int, PROTGeneralesEdit, PROTGenerales>(GeneralesId, EditModel, "PROTN04", registro.RazonSocial, RegistroId);
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ToString(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            PageOperationResult result = new();

            // Validate data
            if (await _protservice.InvalidItemDataAsync<int, PROTGenerales, PROTGeneralesEdit>(GeneralesId))
            {
                result.Message = _options.Value.GrandesClientes.Strings.NoAprobar;
                result.Success = false;
                return new JsonResult(result);
            }

            result = await _protservice.ApproveAsync<int, PROTGenerales>(GeneralesId, RegistroId, "PROTN04");
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ToString(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }
    }
}

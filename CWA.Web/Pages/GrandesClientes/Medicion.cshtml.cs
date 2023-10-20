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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace CWA.Web.Pages.GrandesClientes
{
    [Authorize(Policy = PolicyNames.GCMedicionPolicy)]
    public class MedicionModel : PageModel
    {
        // Services
        private readonly GCService _gcdata;
        private readonly GCAccessControlService _gccontrol;
        private readonly AgenteService _agente;
        private readonly CatalogoService _catalogo;
        private readonly IOptions<ApplicationSettings> _options;

        // Models
        [BindProperty]
        public GCMedicionEdit EditModel { get; set; }

        public GCMedicionView ViewModel { get; set; }

        public List<SelectListItem> Tipos = new();
        public List<CatalogoListing> Identificaciones = new();
        public List<SelectListItem> Distribuidoras = new();
        public List<SelectListItem> Subestaciones = new();
        public List<SelectListItem> Lineas = new();
        public List<SelectListItem> Tensiones = new();

        // Routing
        [BindProperty(SupportsGet = true)]
        public int RegistroId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MedicionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Handler { get; set; }

        public string HandlerName = "";

        // Presentation
        public bool ShowView = false;
        public bool ShowUpdate = false;
        public bool ShowApprove = false;
        public string Nombre = "";
        public DateTime? LastSave;

        public ViewPack<object> RegistroContext;

        public MedicionModel(GCService DataService, GCAccessControlService ControlService, AgenteService Agente, CatalogoService Catalogo, IOptions<ApplicationSettings> Options)
        {
            _gcdata = DataService;
            _gccontrol = ControlService;
            _catalogo = Catalogo;
            _agente = Agente;
            _options = Options;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            RegistroContext = (MedicionId is null)
                ? await _gcdata.ViewRegistroContextAsync(RegistroId)
                : await _gcdata.ViewRegistroContextAsync<int, GCMedicion>(RegistroId, MedicionId.Value);

            Nombre = $"{RegistroContext["nombre"]}";

            if (MedicionId is null)
            {
                ShowUpdate = true;
                RegistroContext.Add("ltext", "Nuevo registro de medición");
                RegistroContext.Add("rtext", "");

                await LoadLists();
            }
            else
            {
                LastSave = (DateTime)RegistroContext["modfecha"];

                ShowUpdate = await _gccontrol.AllowMedicionPostAsync(RegistroId, MedicionId, GCConstants.UPDATE);

                if (ShowUpdate)
                {
                    HandlerName = "Update";
                    EditModel = await _gcdata.GetDatosAsync<int, GCMedicion, GCMedicionEdit>(MedicionId.Value);
                    await LoadLists();
                }
                else
                {
                    ShowView = true;
                    ViewModel = await _gcdata.ViewDatosAsync<int, GCMedicion, GCMedicionView>(MedicionId.Value);
                }
            }

            ShowApprove = MedicionId.HasValue && await _gccontrol.AllowMedicionPostAsync(RegistroId, MedicionId, GCConstants.APPROVE);

            return Page();
        }

        private async Task LoadLists()
        {
            Tipos = (await _catalogo.GetCatalogoListingAsync("GCCONEXION"))
                .OrderBy(o => o.RefVal5)
                .Select(s => new SelectListItem { Value = s.Id, Text = s.Descripcion })
                .ToList();
            

            Tensiones = (await _catalogo.GetCatalogoListingAsync("TENSION"))
                .OrderBy(o => o.RefVal5)
                .Select(s => new SelectListItem { Value = s.Id, Text = s.Descripcion })
                .ToList();

            Distribuidoras = await _agente.GetForSelectByTipoAsync("TAG02");
            Identificaciones = await _catalogo.GetCatalogoListingAsync("TIPODISTIDENTIFICACION");
            Subestaciones = await _catalogo.GetForSelectByGrupoAsync("SUBESTACION");
            Lineas = await _catalogo.GetForSelectByGrupoAsync("LINEA");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PageOperationResult result = new();

            if (!ModelState.IsValid)
            {
                result.Message = _options.Value.Strings.DatosInvalidos;
                result.Success = false;
                result.Content = ModelState.GetErrorMessages();
                return new JsonResult(result);
            }

            // Validate Distribuidora
            if (!EditModel.DistribuidoraId.Empty())
            {
                if (EditModel.TipoDistIdentificacionId.Empty() || EditModel.Identificacion.Empty())
                {
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Success = false;
                    result.Content = new { Key = "EditModel-Identificacion", Message = $"{result.Message}" };
                    return new JsonResult(result);
                }
            }

            // Validate conexion SIN
            if (EditModel.TipoConexionId == "GCC01")
            {
                if (EditModel.SubestacionId.Empty())
                {
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Success = false;
                    result.Content = new { Key = "EditModel-SubestacionId", Message = $"{result.Message}" };
                    return new JsonResult(result);
                }

                if (EditModel.LineaId.Empty())
                {
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Success = false;
                    result.Content = new { Key = "EditModel-LineaId", Message = $"{result.Message}" };
                    return new JsonResult(result);
                }
            }

            // Validate conexion Distribución
            if ((new string[] { "GCC02", "GCC03" }).ToList().Contains(EditModel.TipoConexionId))
            {
                if (EditModel.DistribuidoraId.Empty())
                {
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Success = false;
                    result.Content = new { Key = "EditModel-DistribuidoraId", Message = $"{result.Message}" };
                    return new JsonResult(result);
                }

                if (EditModel.Circuito.Empty())
                {
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Success = false;
                    result.Content = new { Key = "EditModel-Circuito", Message = $"{result.Message}" };
                    return new JsonResult(result);
                }
            }

            result = (MedicionId is null)
                ? await _gcdata.CreateMedicionAsync(RegistroId, EditModel, "GCN06")
                : await _gcdata.SaveDatosAsync<int, GCMedicionEdit, GCMedicion>(MedicionId.Value, EditModel, "GCN06", RegistroId);

            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ApplyFormat(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            var medicion = await _gcdata.GetDatosAsync<int, GCMedicion, GCMedicionEdit>(MedicionId.Value);
            bool smec = (new string[] { "GCC01", "GCC02" }).ToList().Contains(medicion.TipoConexionId);

            PageOperationResult result = await _gcdata.ApproveAsync<int, GCMedicion>(MedicionId.Value, RegistroId, "GCN06", AddMedidorDist: !smec, AddDocumentoSmec: smec);
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ApplyFormat(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl;
using CWA.AccessControl.Authorization.GrandesClientes;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
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
using Microsoft.Extensions.Options;

namespace CWA.Web.Pages.GrandesClientes
{
    [Authorize(Policy = PolicyNames.GCMedidorDistPolicy)]
    public class MedidorDistModel : PageModel
    {
        // Services
        private readonly GCService _gcdata;
        private readonly CatalogoService _catalogos;
        private readonly GCAccessControlService _gccontrol;
        private readonly IOptions<ApplicationSettings> _options;

        // Models
        [BindProperty]
        public GCMedidorDistEdit EditModel { get; set; }

        public GCMedidorDistView ViewModel { get; set; }

        // Lists
        public List<SelectListItem> Fabricantes;
        public List<CatalogoListing> Modelos;
        public List<SelectListItem> FabricanteModelo;
        public List<SelectListItem> CanalDescripcion;

        // Routing
        [BindProperty(SupportsGet = true)]
        public int RegistroId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int MedidorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Handler { get; set; }

        public string HandlerName = "";

        // Presentation
        public bool ShowView = false;
        public bool ShowUpdate = false;
        public bool ShowApprove = false;
        public string Nombre = "";
        public DateTime LastSave;

        public ViewPack<object> RegistroContext;

        public MedidorDistModel(GCService DataService, CatalogoService Catalogos, GCAccessControlService ControlService, IOptions<ApplicationSettings> Options)
        {
            _gcdata = DataService;
            _catalogos = Catalogos;
            _gccontrol = ControlService;
            _options = Options;

            Fabricantes = new();
            Modelos = new();
            FabricanteModelo = new();
            CanalDescripcion = new();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            RegistroContext = await _gcdata.ViewRegistroContextAsync<int, GCMedidorDist>(RegistroId, MedidorId);
            RegistroContext.Add("ltext", $"Medidor {RegistroContext["dist"]} ({RegistroContext["serie"]})");

            Nombre = $"{RegistroContext["nombre"]}";
            LastSave = (DateTime)RegistroContext["modfecha"];

            ShowUpdate = await _gccontrol.AllowMedidorDistPostAsync(RegistroId, MedidorId, GCConstants.UPDATE);

            if (ShowUpdate)
            {
                EditModel = await _gcdata.GetDatosAsync<int, GCMedidorDist, GCMedidorDistEdit>(MedidorId);
                await LoadLists(EditModel.FabricanteId);
            }
            else
            {
                ShowView = true;
                ViewModel = await _gcdata.ViewDatosAsync<int, GCMedidorDist, GCMedidorDistView>(MedidorId);
            }

            ShowApprove = await _gccontrol.AllowMedidorDistPostAsync(RegistroId, MedidorId, GCConstants.APPROVE);

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

            result = await _gcdata.SaveDatosAsync<int, GCMedidorDistEdit, GCMedidorDist>(MedidorId, EditModel, "GCN06", RegistroId);
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ApplyFormat(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            PageOperationResult result = new();

            // Validate data
            if (await _gcdata.InvalidItemDataAsync<int, GCMedidorDist, GCMedidorDistEdit>(MedidorId))
            {
                result.Message = _options.Value.GrandesClientes.Strings.NoAprobar;
                result.Success = false;
                return new JsonResult(result);
            }

            result = await _gcdata.ApproveAsync<int, GCMedidorDist>(MedidorId, RegistroId, "GCN06");
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ApplyFormat(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }

        public async Task LoadLists(string FabricanteId)
        {
            Fabricantes = (await _catalogos.GetForSelectByGrupoAsync("MEDIDORFABRICANTE"))
                .OrderBy(o => o.Text)
                .ToList();

            Modelos = (await _catalogos.GetByFilterAsync(Grupo: "MEDIDORMODELO"))
                .OrderBy(o => o.RefVal1)
                .ThenBy(t => t.Descripcion)
                .ToList();

            CanalDescripcion = (await _catalogos.GetForSelectByGrupoAsync(Grupo: "MEDIDORCANAL")).
                OrderBy(o => o.Text)
                .ToList();

            // Preload fabricante -> modelos list if value set
            if (!FabricanteId.Empty())
            {
                var lookup = (await _catalogos.GetByFilterAsync(Grupo: "MEDIDORMODELO", RefVal1: FabricanteId)).OrderBy(o => o.Descripcion).ToList();

                foreach (var item in lookup) FabricanteModelo.Add(new() { Value = item.Id, Text = item.Descripcion });
            }
        }
    }
}
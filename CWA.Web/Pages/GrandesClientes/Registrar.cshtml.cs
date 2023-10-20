using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl.Constants;
using CWA.Application.Services.Comun;
using CWA.Application.Services.GrandesClientes;
using CWA.Application.Services.Identity;
using CWA.Entities.GrandesClientes;
using CWA.Models.GrandesClientes.Edit;
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
    [Authorize(Policy = PolicyNames.GCRegistrarPolicy)]
    public class RegistrarModel : PageModel
    {
        // Services        
        private readonly GCService _gcdata;
        private readonly AppUserService _usuario;
        private readonly AgenteService _agente;
        private readonly CatalogoService _catalogo;
        private readonly IOptions<ApplicationSettings> _options;

        // Models
        [BindProperty]
        public GCRegistroEdit EditModel { get; set; }

        // Routing
        [BindProperty(SupportsGet = true)]
        public int? RegistroId { get; set; }

        // Lists
        public List<SelectListItem> Propietarios = new();
        public List<SelectListItem> Responsables = new();
        public List<SelectListItem> Tipos = new();

        public RegistrarModel(GCService DataService, AppUserService Usuario, AgenteService Agente, CatalogoService Catalogo, IOptions<ApplicationSettings> Options)
        {
            _gcdata = DataService;
            _usuario = Usuario;
            _agente = Agente;
            _catalogo = Catalogo;
            _options = Options;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            EditModel = (RegistroId.HasValue) ? await _gcdata.GetDatosAsync<int, GCRegistro, GCRegistroEdit>(RegistroId.Value) : new();

            Propietarios = await _usuario.GetForSelectByClaim(ClaimNames.GCSection, new[] { GroupNames.ExternGroup }, false);
            Responsables = await _agente.GetForSelectByTipoAsync("TAG01");
            Tipos = await _catalogo.GetForSelectByGrupoAsync("GCTIPO");
            Tipos = Tipos.OrderBy(o => o.Value).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PageOperationResult result = new();

            if (!ModelState.IsValid)
            {
                result.Success = false;
                result.Message = _options.Value.Strings.DatosInvalidos;
                result.Content = ModelState.GetErrorMessages();

                return new JsonResult(result);
            }
            else
            {
                if (EditModel.TipoGranClienteId == "GCT01")
                {
                    EditModel.PropietarioId = null;

                    if (EditModel.ResponsableId.Empty())
                    {
                        result.Success = false;
                        result.Message = _options.Value.Strings.DatosInvalidos;
                        result.Content = new { Key = "EditModel-ResponsableId", Message = "The field Usuario Propietario is required." };

                        return new JsonResult(result);
                    }
                }
                else
                {
                    EditModel.ResponsableId = null;

                    if (EditModel.PropietarioId.Empty())
                    {
                        result.Success = false;
                        result.Message = _options.Value.Strings.DatosInvalidos;
                        result.Content = new { Key = "EditModel-PropietarioId", Message = "The field Usuario Propietario is required." };

                        return new JsonResult(result);
                    }
                }
            }            

            result = await _gcdata.SaveRegistroAsync(EditModel, "GCN01", RegistroId);

            return new JsonResult(result);
        }
    }
}

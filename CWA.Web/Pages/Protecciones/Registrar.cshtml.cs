using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl.Constants;
using CWA.Application.Services.Comun;
using CWA.Application.Services.Identity;
using CWA.Application.Services.Protecciones;
using CWA.Entities.Protecciones;
using CWA.Models.Identity;
using CWA.Models.Protecciones.Edit;
using CWA.Shared.Helpers;
using CWA.Web.Extensions;
using CWA.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace CWA.Web.Pages.Protecciones
{
    [Authorize(Policy = PolicyNames.PROTRegistrarPolicy)]
    public class RegistrarModel : PageModel
    {        // Services        
        private readonly PROTService _protservice;
        private readonly AppUserService _usuario;
        private readonly AgenteService _agente;
        private readonly CatalogoService _catalogo;
        private readonly IOptions<ApplicationSettings> _options;

        // Models
        [BindProperty]
        public PROTRegistroEdit EditModel { get; set; }

        // Routing
        [BindProperty(SupportsGet = true)]
        public int? RegistroId { get; set; }

        // Lists
        public List<SelectListItem> Usuarios = new();
        public List<SelectListItem> Agentes = new();
        public List<SelectListItem> Tipos = new();

        public RegistrarModel(PROTService Svc, AppUserService Usuario, AgenteService Agente, CatalogoService Catalogo, IOptions<ApplicationSettings> Options)
        {
            _protservice = Svc;
            _usuario = Usuario;
            _agente = Agente;
            _catalogo = Catalogo;
            _options = Options;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            EditModel = (RegistroId.HasValue) ? await _protservice.GetDatosAsync<int, PROTRegistro, PROTRegistroEdit>(RegistroId.Value) : new();

            Usuarios = await _usuario.GetForSelectByClaim(ClaimNames.PROTSection, new[] { GroupNames.ExternGroup });
            Tipos = await _catalogo.GetForSelectByGrupoRefAsync("TIPOGENERADOR", "1");

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
            }

            result = await _protservice.SaveRegistroAsync(EditModel, "PROTN01", RegistroId);

            return new JsonResult(result);
        }
    }
}

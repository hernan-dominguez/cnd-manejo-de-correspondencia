using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Comun;
using CWA.Application.Services.ViabilidadContratos;
using CWA.Entities.ViabilidadContratos;
using CWA.Models.Comun;
using CWA.Models.ViabilidadContratos.Edit;
using CWA.Models.ViabilidadContratos.View;
using CWA.Shared.Extensions;
using CWA.Shared.Helpers;
using CWA.Web.Extensions;
using CWA.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace CWA.Web.Pages.ViabilidadContratos
{
    [Authorize(Policy = PolicyNames.VCNacionalPolicy)]
    public class NacionalModel : PageModel
    {
        // Services
        private readonly VCService _vcdata;
        private readonly AppAccessControlService _access;
        private readonly VCAccessControlService _vccontrol;
        private readonly AgenteService _agente;
        private readonly CatalogoService _catalogo;
        private readonly IOptions<ApplicationSettings> _options;

        // Models
        [BindProperty]
        public VCNacionalEdit EditModel { get; set; }

        [BindProperty]
        [Display(Name = "Motivo del rechazo")]
        public string MotivoRechazo { get; set; }

        public VCNacionalView ViewModel { get; set; }

        // Lists
        public List<CatalogoListing> Tipos = new();
        public AgenteListing UsuarioGen = new();
        public AgenteListing UsuarioDist = new();
        public List<SelectListItem> Distribuidoras = new();
        public List<SelectListItem> Generadores = new();
        // public List<SelectListItem> GClientes = new();
        // public List<ResponsableGranClienteListing> Responsables = new();
        public List<CatalogoListing> DocTipos = new();
        public List<CatalogoListing> Documentos = new();

        // Routing
        [BindProperty(SupportsGet = true)]
        public int? RegistroId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? DocumentoId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Handler { get; set; }

        // Presentation
        public bool ShowView = false;
        public bool ShowUpdate = false;
        public bool ShowApprove = false;

        public ViewPack<object> RegistroContext;

        public NacionalModel(VCService DataService, VCAccessControlService ControlService, AppAccessControlService Access, 
            AgenteService Agente, CatalogoService Catalogo, IOptions<ApplicationSettings> Options)
        {
            _vcdata = DataService;
            _vccontrol = ControlService;
            _access = Access;
            _catalogo = Catalogo;
            _agente = Agente;
            _options = Options;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get related info
            RegistroContext = await _vcdata.ViewRegistroContext<VCNacional>(RegistroId);

            if (RegistroId is null)
            {
                ShowUpdate = true;
                RegistroContext.Add("update", "Información del Contrato");

                await LoadLists();
            }
            else
            {
                ShowView = true;
                ViewModel = await _vcdata.ViewNacionalAsync(RegistroId.Value);
                ShowApprove = await _vccontrol.AllowRegistroPostAsync(VCConstants.NACIONAL, RegistroId, VCConstants.APPROVE);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadAsync()
        {
            // Get the full physical path, or empty string
            VCDocumentoDownload document = await _vcdata.ViewDocumentoAsync<VCDocNacional, VCNacional>(RegistroId.Value, DocumentoId.Value, _options.Value.ViabilidadContratos.DocumentsRoot);

            if (document.FilePath.Empty()) return new StatusCodeResult(404);

            // Set response headers
            var provider = new FileExtensionContentTypeProvider();
            var contentDispositionHeader = new ContentDispositionHeaderValue("inline");
            var cacheHeader = new CacheControlHeaderValue();

            contentDispositionHeader.SetHttpFileName(document.FileName);
            cacheHeader.NoCache = true;

            if (provider.TryGetContentType(document.FilePath, out string contentType))
            {
                Response.Headers[HeaderNames.ContentDisposition] = contentDispositionHeader.ToString();
                Response.Headers[HeaderNames.CacheControl] = cacheHeader.ToString();

                return PhysicalFile(document.FilePath, contentType);
            }

            // If we got here, something went wrong
            return new StatusCodeResult(404);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PageOperationResult result = new();

            if (!ModelState.IsValid)
            {
                List<object> modelErrors = (List<object>)ModelState.GetErrorMessages();

                // Also check file extensions for submitted files
                if (EditModel.Documentos is not null)
                {
                    foreach (var documento in EditModel.Documentos.Where(w => w.DocumentUpload is not null))
                    {
                        if (await _vcdata.InvalidFileExtensionAsync(documento.DocumentUpload, documento.TipoDocumentoId))
                        {
                            modelErrors.Add(new
                            {
                                Key = $"EditModel-Documentos-{EditModel.Documentos.IndexOf(documento)}-DocumentUpload",
                                Message = _options.Value.Strings.ArchivoExtension
                            });
                        }
                    }
                }

                result.Success = false;
                result.Message = _options.Value.Strings.DatosInvalidos;
                result.Content = modelErrors;

                return new JsonResult(result);
            }

            // Validate dates
            int validationCode = await _vcdata.InvalidDatesAsync(EditModel.Inicia.Date, EditModel.Finaliza.Date);

            if (validationCode != 0)
            {
                var datesError = new List<object>
                {
                    new { Key = "EditModel-Inicia", Message = _options.Value.Strings.FechaInvalida },
                    new { Key = "EditModel-Finaliza", Message = _options.Value.Strings.FechaInvalida }
                };

                result.Success = false;
                result.Message = validationCode == -1 ? _options.Value.Strings.DatosInvalidos : _options.Value.Strings.PeriodoInvalido;
                result.Content = datesError;

                return new JsonResult(result);
            }

            // Validate file extensions
            var extensionErrors = new List<object>();

            foreach (var documento in EditModel.Documentos)
            {
                if (await _vcdata.InvalidFileExtensionAsync(documento.DocumentUpload, documento.TipoDocumentoId))
                {
                    extensionErrors.Add(new
                    {
                        Key = $"EditModel-Documentos-{EditModel.Documentos.IndexOf(documento)}-DocumentUpload",
                        Message = _options.Value.Strings.ArchivoExtension
                    });
                }
            }

            if (extensionErrors.Count > 0)
            {
                result.Success = false;
                result.Message = _options.Value.Strings.DatosInvalidos;
                result.Content = extensionErrors;

                return new JsonResult(result);
            }

            // Proceed
            EditModel.DocsPath = _options.Value.ViabilidadContratos.DocumentsRoot;
            EditModel.TempPath = _options.Value.ViabilidadContratos.DocumentsTemp;
            EditModel.NotificacionId = "VCN01";

            result = await _vcdata.SaveNacionalAsync(EditModel);

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            var AtencionModel = new VCAtencion { RegistroId = RegistroId.Value, Aprobacion = true, NotificacionId = "VCN04" };
                        
            PageOperationResult result = await _vcdata.ApprovalNacionalAsync(AtencionModel);

            result.TimeString = result.TimeValue.HasValue ? result.TimeValue.Value.ApplyFormat(_options.Value.DateFormat) : result.TimeString;

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostRejectAsync()
        {
            PageOperationResult result = new();

            if (MotivoRechazo.Empty())
            {
                result.Success = false;
                result.Message = _options.Value.Strings.DatosInvalidos;
                result.Content = new List<object> { new { Key = "Model-MotivoRechazo", Message = "El campo Motivo del Rechazo es requerido." } };

                return new JsonResult(result);
            }

            // Proceed
            var AtencionModel = new VCAtencion { RegistroId = RegistroId.Value, Aprobacion = false, NotificacionId = "VCN04", Motivo = MotivoRechazo };

            result = await _vcdata.ApprovalNacionalAsync(AtencionModel);

            result.TimeString = result.TimeValue.HasValue ? result.TimeValue.Value.ApplyFormat(_options.Value.DateFormat) : result.TimeString;

            return new JsonResult(result);
        }

        private async Task LoadLists()
        {
            string userGroup = await _access.GetGroupAsync();

            if (userGroup == GroupNames.ExternGroup)
            {
                // Get tipos
                Tipos = (await _catalogo.GetByFilterAsync(Grupo: "VCCONTRATO"))
                    .Where(w => w.RefVal3 == $"{RegistroContext["agt-tipo"]}" || w.RefVal4 == $"{RegistroContext["agt-tipo"]}")
                    .OrderBy(o => o.RefVal5)
                    .ToList();

                // Get participants
                if ($"{RegistroContext["agt-tipo"]}" == "TAG01") UsuarioGen = await _agente.GetByUserAsync();
                if ($"{RegistroContext["agt-tipo"]}" == "TAG02") UsuarioDist = await _agente.GetByUserAsync();

                Generadores = (await _agente.GetByTipoAsync<AgenteListing>("TAG01"))
                    .Where(w => w.Id != UsuarioGen.Id)
                    .Select(t => new SelectListItem { Text = t.Nombre, Value = $"{t.Id}" })
                    .OrderBy(o => o.Text)
                    .ToList();

                Distribuidoras = (await _agente.GetByTipoAsync<AgenteListing>("TAG02"))
                    .Where(w => w.Id != UsuarioDist.Id)
                    .Select(t => new SelectListItem { Text = t.Nombre, Value = $"{t.Id}" })
                    .OrderBy(o => o.Text)
                    .ToList();

                //GClientes = (await _agente.GetGrandesClientesByUserAsync<SelectListItem>())
                //    .OrderBy(o => o.Text)
                //    .ToList();
            }

            if (userGroup == GroupNames.AdminGroup)
            {
                // Get Tipos
                Tipos = (await _catalogo.GetByFilterAsync(Grupo: "VCCONTRATO"))
                    .OrderBy(o => o.RefVal5)
                    .ToList();

                // Get participants
                Generadores = (await _agente.GetByTipoAsync<AgenteListing>("TAG01"))
                    .Where(w => w.Id != UsuarioGen.Id)
                    .Select(t => new SelectListItem { Text = t.Nombre, Value = $"{t.Id}" })
                    .OrderBy(o => o.Text)
                    .ToList();

                Distribuidoras = (await _agente.GetByTipoAsync<AgenteListing>("TAG02"))
                    .Where(w => w.Id != UsuarioDist.Id)
                    .Select(t => new SelectListItem { Text = t.Nombre, Value = $"{t.Id}" })
                    .OrderBy(o => o.Text)
                    .ToList();

                //Responsables = await _agente.GetResponsablesGrandesClientesAsync();
            }

            // Documentos
            if (userGroup == GroupNames.ExternGroup || userGroup == GroupNames.AdminGroup)
            {
                DocTipos = (await _catalogo.GetByFilterAsync(Grupo: "VCDOCXTIPO"))
                    .Where(w => w.RefVal1.Contains("VCC"))
                    .ToList();

                var lookup = DocTipos.Select(s => s.RefVal2).ToList();

                Documentos = (await _catalogo.GetByFilterAsync(Grupo: "VCDOCUMENTO"))
                    .Where(w => lookup.Contains(w.Id))
                    .ToList();

                // Apply document sorting
                DocTipos = DocTipos.Select(t => new CatalogoListing
                {
                    Id = t.Id,
                    Grupo = t.Grupo,
                    Descripcion = t.Descripcion,
                    RefVal1 = t.RefVal1,
                    RefVal2 = t.RefVal2,
                    RefVal3 = Documentos.Where(w => w.Id == t.RefVal2).First().RefVal5

                }).OrderBy(o => o.RefVal1).ThenByDescending(t => t.RefVal3).ToList();
            }
        }
    }
}

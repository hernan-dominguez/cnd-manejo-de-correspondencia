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
    [Authorize(Policy = PolicyNames.VCEnmiendaPolicy)]
    public class EnmiendaModel : PageModel
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
        public VCEnmiendaEdit EditModel { get; set; }

        [BindProperty]
        [Display(Name = "Motivo del rechazo")]
        public string MotivoRechazo { get; set; }

        public VCEnmiendaView ViewModel { get; set; }

        // Lists
        public List<VCNacionalView> Contratos = new();
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

        public ViewPack<object> ViewContext;

        public EnmiendaModel(VCService DataService, VCAccessControlService ControlService, AppAccessControlService Access, 
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
            ViewContext = await _vcdata.ViewRegistroContext<VCEnmienda>(RegistroId);

            if (RegistroId is null)
            {
                ShowUpdate = true;
                ViewContext.Add("update", "Información del Contrato");

                await LoadLists();
            }
            else
            {
                ShowView = true;
                ViewModel = await _vcdata.ViewEnmiendaAsync(RegistroId.Value);
                ShowApprove = await _vccontrol.AllowRegistroPostAsync(VCConstants.ENMIENDA, RegistroId, VCConstants.APPROVE);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadAsync()
        {
            // Get the full physical path, or empty string
            VCDocumentoDownload document = await _vcdata.ViewDocumentoAsync<VCDocEnmienda, VCEnmienda>(RegistroId.Value, DocumentoId.Value, _options.Value.ViabilidadContratos.DocumentsRoot);

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
            EditModel.NotificacionId = "VCN03";

            result = await _vcdata.SaveEnmiendaAsync(EditModel);

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            var AtencionModel = new VCAtencion { RegistroId = RegistroId.Value, Aprobacion = true, NotificacionId = "VCN06" };

            PageOperationResult result = await _vcdata.ApprovalEnmiendaAsync(AtencionModel);

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
            var AtencionModel = new VCAtencion { RegistroId = RegistroId.Value, Aprobacion = false, NotificacionId = "VCN06", Motivo = MotivoRechazo };

            result = await _vcdata.ApprovalEnmiendaAsync(AtencionModel);

            result.TimeString = result.TimeValue.HasValue ? result.TimeValue.Value.ApplyFormat(_options.Value.DateFormat) : result.TimeString;

            return new JsonResult(result);
        }

        private async Task LoadLists()
        {
            string userGroup = await _access.GetGroupAsync();

            if (userGroup == GroupNames.ExternGroup)
            {
                // Get contratos
                Contratos = (await _vcdata.ViewNacionalesAsync((int)ViewContext["agt-id"], true))
                    .OrderBy(o => o.Codigo)
                    .ToList();
            }

            if (userGroup == GroupNames.AdminGroup)
            {
                // Get contratos
                Contratos = (await _vcdata.ViewNacionalesAsync(Aprobados: true))
                    .OrderBy(o => o.Codigo)
                    .ToList();
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

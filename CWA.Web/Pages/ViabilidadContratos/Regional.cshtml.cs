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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace CWA.Web.Pages.ViabilidadContratos
{
    [Authorize(Policy = PolicyNames.VCRegionalPolicy)]
    public class RegionalModel : PageModel
    {
        // Services
        private readonly VCService _vcdata;
        private readonly AppAccessControlService _access;
        private readonly VCAccessControlService _vccontrol;
        private readonly AgenteService _agente;
        private readonly AgenteRegionalService _regional;
        private readonly CatalogoService _catalogo;
        private readonly IOptions<ApplicationSettings> _options;

        // Models
        [BindProperty]
        public VCRegionalEdit EditModel { get; set; }

        [BindProperty]
        [Display(Name = "Motivo del rechazo")]
        public string MotivoRechazo { get; set; }

        [BindProperty]
        public IFormFile Adjunto { get; set; }
        
        public VCRegionalView ViewModel { get; set; }

        // Lists
        public List<CatalogoListing> TipoSolicitudes = new();
        public List<CatalogoListing> TipoTransacciones = new();
        public List<CatalogoListing> SolicitudTransaccion = new();
        public List<SelectListItem> Solicitantes = new();
        public List<AgenteRegionalListing> Contrapartes = new();
        public List<CatalogoListing> Paises = new();
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
        public bool ShowOwn = false;
        public bool ShowFile = false;        
        public string UploadAccepts = "";
        public string UploadDisplay = "";

        public ViewPack<object> RegistroContext;

        public RegionalModel(VCService DataService, VCAccessControlService ControlService, AppAccessControlService Access, 
            AgenteService Agente, AgenteRegionalService Regional, CatalogoService Catalogo, IOptions<ApplicationSettings> Options)
        {
            _vcdata = DataService;
            _vccontrol = ControlService;
            _access = Access;
            _catalogo = Catalogo;
            _agente = Agente;
            _regional = Regional;
            _options = Options;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get related info
            RegistroContext = await _vcdata.ViewRegistroContext<VCRegional>(RegistroId);

            if (RegistroId is null)
            {
                ShowUpdate = true;
                RegistroContext.Add("update", "Información de la Solicitud");

                await LoadLists();
            }
            else
            {
                ShowView = true;
                ViewModel = await _vcdata.ViewRegionalAsync(RegistroId.Value);
                ShowApprove = await _vccontrol.AllowRegistroPostAsync(VCConstants.REGIONAL, RegistroId, VCConstants.APPROVE);

                // Adjunto
                var specs = (await _catalogo.GetByFilterAsync(Grupo: "VCADJUNTOXTIPO")).First();
                
                if (ShowApprove && ViewModel.TipoSolicitudId == specs.RefVal1)
                {
                    var docSpecs = await _catalogo.GetCatalogoAsync(specs.RefVal2);

                    UploadDisplay = docSpecs.Descripcion;
                    UploadAccepts = $".{docSpecs.RefVal1.Replace(",", ",.")}";                    
                    ShowFile = true;
                }                
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadAsync()
        {
            // Get the full physical path, or empty string
            VCDocumentoDownload document = await _vcdata.ViewDocumentoAsync<VCDocRegional, VCRegional>(RegistroId.Value, DocumentoId.Value, _options.Value.ViabilidadContratos.DocumentsRoot);

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
                            // ToDo: Move file extension message to application settings strings
                            modelErrors.Add(new 
                            { 
                                Key = $"EditModel-Documentos-{EditModel.Documentos.IndexOf(documento)}-DocumentUpload", 
                                Message = _options.Value.GrandesClientes.Strings.ArchivoExtension 
                            });
                        }
                    }
                }

                result.Success = false;
                result.Message = _options.Value.Strings.DatosInvalidos;
                result.Content = modelErrors;

                return new JsonResult(result);
            }

            // Validate file extensions
            var extensionErrors = new List<object>();

            foreach (var documento in EditModel.Documentos)
            {
                if (await _vcdata.InvalidFileExtensionAsync(documento.DocumentUpload, documento.TipoDocumentoId))
                {
                    // ToDo: Move file extension message to application settings strings
                    extensionErrors.Add(new 
                    { 
                        Key = $"EditModel-Documentos-{EditModel.Documentos.IndexOf(documento)}-DocumentUpload", 
                        Message = _options.Value.GrandesClientes.Strings.ArchivoExtension 
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
            EditModel.NotificacionId = "VCN02";

            result = await _vcdata.SaveRegionalAsync(EditModel);

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            PageOperationResult result = new();

            // Check if file is required
            string tipoSolicitudId = (await _vcdata.ViewRegionalAsync(RegistroId.Value)).TipoSolicitudId;
            var specs = (await _catalogo.GetByFilterAsync(Grupo: "VCADJUNTOXTIPO")).First();

            if (specs.RefVal1 == tipoSolicitudId)
            {
                if (Adjunto is null)
                {
                    result.Success = false;
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Content = new List<object> { new { Key = "Model-Adjunto", Message = _options.Value.Strings.DatosInvalidos } };

                    return new JsonResult(result);
                }

                // Validate file extension
                if (await _vcdata.InvalidFileExtensionAsync(Adjunto, specs.RefVal2))
                {
                    result.Success = false;
                    result.Message = _options.Value.Strings.DatosInvalidos;
                    result.Content = new List<object> { new { Key = "Model-Adjunto", Message = _options.Value.Strings.DatosInvalidos } };

                    return new JsonResult(result);
                }
            }

            // Proceed
            var AtencionModel = new VCAtencion 
            { 
                RegistroId = RegistroId.Value, 
                Aprobacion = true, 
                NotificacionId = "VCN05" ,                
                TempPath = _options.Value.ViabilidadContratos.DocumentsTemp,
                MailPath = _options.Value.ViabilidadContratos.AttachmentsRoot,
                DocsPath = _options.Value.ViabilidadContratos.DocumentsRoot,
                Adjunto = Adjunto
            };           

            result = await _vcdata.ApprovalRegionalAsync(AtencionModel);

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
            var AtencionModel = new VCAtencion { RegistroId = RegistroId.Value, Aprobacion = false, NotificacionId = "VCN05", Motivo = MotivoRechazo };

            result = await _vcdata.ApprovalRegionalAsync(AtencionModel);

            result.TimeString = result.TimeValue.HasValue ? result.TimeValue.Value.ApplyFormat(_options.Value.DateFormat) : result.TimeString;

            return new JsonResult(result);
        }

        private async Task LoadLists()
        {
            string userGroup = await _access.GetGroupAsync();
            
            if ((userGroup == GroupNames.ExternGroup && $"{RegistroContext["agt-tipo"]}" == "TAG01") || userGroup == GroupNames.AdminGroup)
            {
                // Get catalogos
                TipoSolicitudes = (await _catalogo.GetByFilterAsync(Grupo: "VCSOLICITUD"))
                    .OrderBy(o => o.RefVal5)
                    .ToList();

                TipoTransacciones = (await _catalogo.GetByFilterAsync(Grupo: "VCTRANSACCION"))
                    .OrderBy(o => o.RefVal5)
                    .ToList();

                SolicitudTransaccion = await _catalogo.GetCatalogoListingAsync("VCSOLTRAN");

                Paises = (await _catalogo.GetByFilterAsync("PAISREGIONAL"))
                    .OrderBy(o => o.RefVal1)
                    .ToList();

                // Agentes regionales
                Contrapartes = await _regional.GetAgenteRegionalListAsync<AgenteRegionalListing>();
                Contrapartes.ForEach(f => f.Nombre = $"{f.Nombre} ({f.Codigo})");

                // Documentos
                DocTipos = (await _catalogo.GetByFilterAsync(Grupo: "VCDOCXTIPO"))
                    .Where(w => w.RefVal1.Contains("VCS"))
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

                // Solicitantes
                if (userGroup == GroupNames.AdminGroup)
                {
                    ShowOwn = true;
                    Solicitantes = (await _agente.GetByTipoAsync<SelectListItem>("TAG01")).OrderBy(o => o.Text).ToList();
                }
            }
        }
    }
}

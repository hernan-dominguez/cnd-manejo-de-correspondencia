using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Comun;
using CWA.Application.Services.GrandesClientes;
using CWA.Entities.Comun;
using CWA.Entities.GrandesClientes;
using CWA.Models.Comun;
using CWA.Models.GrandesClientes.Edit;
using CWA.Models.GrandesClientes.View;
using CWA.Shared.Helpers;
using CWA.Shared.Extensions;
using CWA.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using CWA.Web.Extensions;
using CWA.AccessControl.Authorization.GrandesClientes;

namespace CWA.Web.Pages.GrandesClientes
{
    [Authorize(Policy = PolicyNames.GCDocumentosPolicy)]
    public class DocumentosModel : PageModel
    {
        // Services
        private readonly GCService _gcdata;
        private readonly GCAccessControlService _gccontrol;
        private readonly AppAccessControlService _access;
        private readonly IOptions<ApplicationSettings> _options;

        // Models
        [BindProperty]
        public GCDocumentoEdit EditModel { get; set; }

        public List<GCDocumentosView> ViewModel { get; set; }
        public List<GCDocumentoSeleccion> Selected { get; set; }

        // Lists
        public List<GCDocumentoList> DocumentosDisponibles = new();

        // Routing
        [BindProperty(SupportsGet = true)]
        public int RegistroId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? DocumentoId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Handler { get; set; }

        // Presentation
        public bool ShowUpdate = false;
        public bool ShowApprove = false;
        public bool ShowSelect = false;
        public bool ShowTemplate = false;
        public bool UserApprove = false;
        public string Nombre = "";

        public ViewPack<object> RegistroContext;

        // Options
        readonly string DocumentsRoot;
        readonly string DocumentsTemp;
        readonly string TemplatesPath;

        public DocumentosModel(GCService DataService, GCAccessControlService ControlService, AppAccessControlService Access, IOptions<ApplicationSettings> Options)
        {
            _gcdata = DataService;
            _gccontrol = ControlService;
            _access = Access;
            _options = Options;

            DocumentsRoot = _options.Value.GrandesClientes.DocumentsRoot;
            DocumentsTemp = _options.Value.GrandesClientes.DocumentsTemp;
            TemplatesPath = _options.Value.GrandesClientes.TemplatesPath;

            Selected = new();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            RegistroContext = await _gcdata.ViewRegistroContextAsync(RegistroId);
            RegistroContext.Add("current", "documentos");

            Nombre = $"{RegistroContext["nombre"]}";

            ViewModel = await _gcdata.ViewDocumentosAsync(RegistroId);
            ShowUpdate = await _gccontrol.AllowDocumentosPostAsync(RegistroId, GCConstants.UPDATE);
            UserApprove = await _access.HasClaimAsync(ClaimNames.GCApprove);
            ShowApprove = UserApprove && ViewModel.Where(w => !w.FechaAtencion.HasValue).Any();

            if (ShowUpdate) DocumentosDisponibles = await _gcdata.ViewDocumentosDisponiblesAsync(RegistroId);

            if (ShowApprove && ViewModel.Where(w => !w.FechaAtencion.HasValue).Any())
            {
                ShowSelect = true;
                ViewModel.ForEach(f => Selected.Add(new GCDocumentoSeleccion { Id = f.Id }));
            }

            // Show template only when uploaded
            GCDocumentoDownload template = await _gcdata.ViewTemplateAsync(RegistroId, _gcdata.DOC_PROY, TemplatesPath);
            ShowTemplate = !template.FilePath.Empty();

            return Page();
        }

        public async Task<IActionResult> OnGetTemplateAsync()
        {
            // Get the full physical path, or empty string
            GCDocumentoDownload template = await _gcdata.ViewTemplateAsync(RegistroId, _gcdata.DOC_PROY, TemplatesPath);

            if (template.FilePath.Empty()) return new StatusCodeResult(404);

            // Set response headers
            var provider = new FileExtensionContentTypeProvider();
            var contentDispositionHeader = new ContentDispositionHeaderValue("attachment");
            var cacheHeader = new CacheControlHeaderValue();

            contentDispositionHeader.SetHttpFileName(template.FileName);
            cacheHeader.NoCache = true;

            if (provider.TryGetContentType(template.FilePath, out string contentType))
            {
                Response.Headers[HeaderNames.ContentDisposition] = contentDispositionHeader.ToString();
                Response.Headers[HeaderNames.CacheControl] = cacheHeader.ToString();

                return PhysicalFile(template.FilePath, contentType);
            }

            // If we got here, something went wrong
            return new StatusCodeResult(404);
        }

        public async Task<IActionResult> OnGetDownloadAsync()
        {
            // Get the full physical path, or empty string
            GCDocumentoDownload document = await _gcdata.ViewDocumentoAsync(RegistroId, DocumentoId.Value, DocumentsRoot);

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

            if (await _gcdata.InvalidFileExtensionAsync(EditModel.DocumentUpload, EditModel.DocumentoId.Value))
            {
                result.Message = _options.Value.GrandesClientes.Strings.ArchivoExtension;
                result.Success = false;
                result.Content = new { Key = "EditModel-DocumentUpload", Message = $"{result.Message}." };
                return new JsonResult(result);
            }

            var registro = await _gcdata.ViewRegistroAsync(RegistroId);

            result = await _gcdata.SaveDocumentoAsync(RegistroId, EditModel, DocumentsRoot, DocumentsTemp, registro.Nombre);

            if (result.Success)
            {
                GCDocumentoResult content = result.Content as GCDocumentoResult;
                content.Actualizado = content.SaveTime.ApplyFormat(_options.Value.DateTimeFormat);
                content.Aprobado = (content.AutoAccepted) ? content.SaveTime.ApplyFormat(_options.Value.DateTimeFormat) : content.Aprobado;
                result.Content = content;
            }

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            PageOperationResult result = new();
                        
            if (DocumentoId is null)
            {
                result.Message = _options.Value.GrandesClientes.Strings.NoAprobar;
                result.Success = false;
                return new JsonResult(result);
            }

            result = await _gcdata.ApproveAsync<int, GCDocumento>(DocumentoId.Value, RegistroId, "", IsDocumento: true);
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ApplyFormat(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }
    }
}

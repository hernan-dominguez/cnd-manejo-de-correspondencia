using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Comun;
using CWA.Application.Services.Protecciones;
using CWA.Entities.Comun;
using CWA.Entities.Protecciones;
using CWA.Models.Comun;
using CWA.Models.Protecciones.Edit;
using CWA.Models.Protecciones.View;
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
using Microsoft.AspNetCore.Identity;
using CWA.Entities.Identity;
using CWA.Shared.Helpers;

namespace CWA.Web.Pages.Protecciones
{
    [Authorize(Policy = PolicyNames.PROTDocumentosPolicy)]
    public class DocumentosModel : PageModel
    {
        // Services
        private readonly PROTService _protservice;
        private readonly PROTAccessControlService _protcontrol;
        private readonly AppAccessControlService _access;
        private readonly IOptions<ApplicationSettings> _options;
        private readonly UserManager<AppUser> _userMgr;

        // Models
        [BindProperty]
        public PROTDocumentoEdit EditModel { get; set; }

        public List<PROTDocumentosView> ViewModel { get; set; }
        public List<PROTDocumentoSeleccion> Selected { get; set; }

        // Lists
        public List<SelectListItem> DocumentosDisponibles = new();

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
        public bool UserApprove = false;
        public bool ShowUpload = false;
        public bool UserUpload = false;
        public string RazonSocial = "";
        public string UsuarioPath = "";

        public ViewPack<object> InformationContext;

        // Options
        readonly string DocumentsRoot;
        readonly string DocumentsTemp;

        public DocumentosModel(PROTService DataService, PROTAccessControlService ControlService,
            AppAccessControlService Access, IOptions<ApplicationSettings> Options, UserManager<AppUser> userMgr)
        {
            _protservice = DataService;
            _protcontrol = ControlService;
            _access = Access;
            _options = Options;
            _userMgr = userMgr;
            DocumentsRoot = _options.Value.Protecciones.DocumentsRoot;
            DocumentsTemp = _options.Value.Protecciones.DocumentsTemp;
            Selected = new();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            InformationContext = await _protservice.ViewInfoAsync(RegistroId);
            RazonSocial = $"{InformationContext["razonsocial"]}";

            ViewModel = await _protservice.ViewDocumentosAsync(RegistroId);
            ShowUpdate = await _protcontrol.AllowDocumentosPostAsync(RegistroId, PROTConstants.UPDATE);
            UserApprove = await _access.HasClaimAsync(ClaimNames.PROTApprove);
            ShowApprove = UserApprove && ViewModel.Where(w => !w.FechaAprobacion.HasValue).Any();

            UserUpload = await _protcontrol.ValidDocumentoOwnership(_access.SessionUserId);
            ShowUpload = ShowUpdate && UserUpload;

            if (ShowUpdate) DocumentosDisponibles = await _protservice.ViewDocumentosDisponiblesAsync(RegistroId);

            if (ShowApprove && ViewModel.Where(w => !w.FechaAprobacion.HasValue).Any())
            {
                ShowSelect = true;
                ViewModel.ForEach(f => Selected.Add(new PROTDocumentoSeleccion { Id = f.Id }));
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadAsync()
        {
            InformationContext = await _protservice.ViewInfoAsync(RegistroId);
            UsuarioPath = $"{InformationContext["usuariopath"]}";
            var pathUsuario = UsuarioPath;

            // Get the full physical path, or empty string
            PROTDocumentoDownload document = await _protservice.ViewDocumentoAsync(pathUsuario, RegistroId, DocumentoId.Value, DocumentsRoot);

            if (document.FilePath.Empty()) return new StatusCodeResult(404);

            // Set response headers
            var provider = new FileExtensionContentTypeProvider();
            var contentDispositionHeader = new ContentDispositionHeaderValue("attachment");
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
            InformationContext = await _protservice.ViewInfoAsync(RegistroId);
            UsuarioPath = $"{InformationContext["usuariopath"]}";

            PageOperationResult result = new();

            if (!ModelState.IsValid)
            {
                result.Message = "Algunos campos no son válidos";
                result.Success = false;
                return new JsonResult(result);
            }

            if (await _protservice.InvalidFileExtension(EditModel.DocumentUpload, EditModel.DocumentoId))
            {
                result.Message = _options.Value.Protecciones.Strings.ArchivoExtension;
                result.Success = false;
                result.Content = new { Key = "EditModel-DocumentUpload", Message = $"{result.Message}." };
                return new JsonResult(result);
            }

            result = await _protservice.SaveDocumentoAsync(UsuarioPath, EditModel, DocumentsRoot, DocumentsTemp);

            if (result.Success)
            {
                PROTDocumentoResult content = result.Content as PROTDocumentoResult;
                content.Actualizado = content.SaveTime.ToString(_options.Value.DateTimeFormat);
                content.Aprobado = (content.AutoAccepted) ? content.SaveTime.ToString(_options.Value.DateTimeFormat) : content.Aprobado;
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

            //PageOperationResult result = await _protservice.ApproveAsync<int, PROTDocumento>(DocumentoId.Value);
            result = await _protservice.ApproveAsync<int, PROTDocumento>(DocumentoId.Value, RegistroId, "", IsDocumento: true);
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ToString(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }
    }
}

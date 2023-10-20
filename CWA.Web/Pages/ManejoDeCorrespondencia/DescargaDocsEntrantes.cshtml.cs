using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Comun;
using CWA.Application.Services.GrandesClientes;
using CWA.Application.Services.ManejoDeCorrespondencia;
using CWA.Entities.Comun;
using CWA.Models.Comun;
using CWA.Models.GrandesClientes.View;
using CWA.Models.ManejoDeCorrespondencia.Carga;
using CWA.Models.ManejoDeCorrespondencia.Descarga;
using CWA.Shared.Extensions;
using CWA.Shared.Helpers;
using CWA.Web.Settings;
using DocumentFormat.OpenXml.Office2010.Word;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Serilog;

namespace CWA.Web.Pages.ManejoDeCorrespondencia
{    
    public class DescargaDocsEntrantes : PageModel
    {
        //Configuration
        private readonly IConfiguration _config;
        private readonly IOptions<ApplicationSettings> _options;

        // Services
        private readonly MCDownloadService _mcDownloadService;
        private readonly AgenteService _agente;

        // Models
        public List<MCArchivoDescarga> ViewModel { get; set; }

        // Lists
        public AgenteListing Usuario = new();

        // Routing
        [BindProperty(SupportsGet = true)]
        public string? DocumentoId { get; set; }

        // Presentation
        public bool CanCreate = false;
        public bool CanView = false;
        public bool ShowMore = false;

        public ViewPack<object> RegistroContext;

        public DescargaDocsEntrantes(MCDownloadService MCDownloadService, AgenteService Agente, IConfiguration Config, IOptions<ApplicationSettings> Options)
        {
            _mcDownloadService = MCDownloadService;
            _agente = Agente;
            _config = Config;
            _options = Options;
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            RegistroContext = new("current", "entrantes");
            Usuario = await _agente.GetByUserAsync();
            var sharepointAuthenticationObject = MCServiceHelpers.GetSharepointAuthenticationObject(_config);
            var startFolder = "/Documentos compartidos/Correspondencia CND/" + Usuario.Codigo + "/Entrante/";
            ViewModel = (await _mcDownloadService.GetAllFilesUnderFolder(startFolder,sharepointAuthenticationObject, Usuario.Codigo)).OrderByDescending(x => x.Fecha).ToList();
            return Page();
        }

        public async Task<IActionResult> OnGetDownloadAsync()
        {
            Usuario = await _agente.GetByUserAsync();
            var sharepointAuthenticationObject = MCServiceHelpers.GetSharepointAuthenticationObject(_config);

            string documentoIdDecoded = HttpUtility.UrlDecode(DocumentoId);
            string spFileName = documentoIdDecoded[(documentoIdDecoded.LastIndexOf('/') + 1)..];
            string spFilePath = documentoIdDecoded.Substring(0, documentoIdDecoded.LastIndexOf(spFileName) - 1);
            string localTempPath = _options.Value.ManejoDeCorrespondencia.DocumentsTemp;

            // Get the full physical path, or empty string
            MCDocumentoDownload document = await _mcDownloadService.DownloadFileFromSharePoint(sharepointAuthenticationObject, spFileName, spFilePath, localTempPath, Usuario.Codigo);

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


    }
}

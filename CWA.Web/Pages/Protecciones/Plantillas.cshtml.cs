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
    [Authorize(Policy = PolicyNames.PROTPlantillasPolicy)]
    public class PlantillasModel : PageModel
    {
        private readonly PROTService _protservice;
        public ViewPack<object> InformationContext;

        // Routing
        [BindProperty(SupportsGet = true)]
        public int RegistroId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PlantillaId { get; set; }

        // Presentation
        public string RazonSocial = "";
        public string UsuarioPath = "";

        // Models
        public List<PROTPlantillasView> ViewModel { get; set; }
        public IOptions<ApplicationSettings> _options { get; }

        // Options
        readonly string PlantillasRoot;
        readonly string PlantillasTemp;

        public PlantillasModel(IOptions<ApplicationSettings> Options, PROTService DataService)
        {
            _options = Options;
            _protservice = DataService;
            PlantillasRoot = _options.Value.Protecciones.PlantillasRoot;
            PlantillasTemp = _options.Value.Protecciones.DocumentsTemp;
        }

        public async Task<IActionResult> OnGet()
        {
            InformationContext = await _protservice.ViewInfoAsync(RegistroId);
            RazonSocial = $"{InformationContext["razonsocial"]}";

            ViewModel = await _protservice.ViewPlantillasAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadAsync()
        {
            // Get the full physical path, or empty string
            PROTPlantillaDownload document = await _protservice.EditPlantillaAsync(PlantillaId.Value, PlantillasRoot, PlantillasTemp, RegistroId);

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

                //Delete the temp file after it was streamed
                return new TempPhysicalFileResult(document.FilePath, contentType);
            }

            // If we got here, something went wrong
            return new StatusCodeResult(404);
        }
    }
}

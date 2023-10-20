using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Protecciones;
using CWA.Models.Protecciones.View;
using CWA.Web.Settings;
using CWA.Entities.Protecciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using CWA.Shared.Helpers;

namespace CWA.Web.Pages.Protecciones
{
    [Authorize(Policy = PolicyNames.PROTRegistroPolicy)]
    public class RegistroModel : PageModel
    {   
        // Services
        private readonly PROTService _protservice;
        private readonly PROTAccessControlService _protcontrol;
        private readonly IOptions<ApplicationSettings> _options;
        private readonly AppAccessControlService _access;

        // Models
        //[Required]
        //[DataType(DataType.Date)]
        //[BindProperty]
        //[Display(Name = "Fecha de Entrada")]
        //public DateTime? FechaEntrada { get; set; }

        public PROTRegistroView ViewModel { get; set; }

        // Routing
        [BindProperty(SupportsGet = true)]
        public int RegistroId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Handler { get; set; }

        // Presentation
        public bool ShowUpdate = false;
        public bool ShowApprove = false;
        public bool ShowUser = false;
        public string RazonSocial = "";
        public string RegUsuario = "";

        public ViewPack<object> InformationContext;

        public RegistroModel(PROTService DataService, PROTAccessControlService ControlService,
            IOptions<ApplicationSettings> Options, AppAccessControlService Access)
        {
            _protservice = DataService;
            _protcontrol = ControlService;
            _options = Options;
            _access = Access;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get related info
            InformationContext = await _protservice.ViewInfoAsync(RegistroId);
            ViewModel = await _protservice.ViewRegistroAsync(RegistroId);
            //ShowUser = ViewModel.Tipo == "Distribuidor";
            ShowUser = await _access.IsInGroup(new string[] { GroupNames.AdminGroup, GroupNames.CndGroup });

            ShowUpdate = await _protcontrol.AllowRegistroPostAsync(RegistroId, PROTConstants.UPDATE);
            ShowApprove = await _protcontrol.AllowRegistroPostAsync(RegistroId, PROTConstants.APPROVE);

            RazonSocial = $"{InformationContext["razonsocial"]}";
            RegUsuario = $"{InformationContext["regusuario"]}";
            //FechaEntrada = ViewModel.FechaEntrada;

            return Page();
        }

        //public async Task<IActionResult> OnPostUpdateAsync()
        //{
        //    PageOperationResult result = new();

        //    if (!ModelState.IsValid)
        //    {
        //        result.Message = "El campo de fecha es requerido";
        //        result.Success = false;
        //        return new JsonResult(result);
        //    }

        //    result = await _protservice.SaveFechaEntradaAsync(RegistroId, FechaEntrada.Value);

        //    return new JsonResult(result);
        //}

        public async Task<IActionResult> OnPostApproveAsync()
        {
            PageOperationResult result = new();

            //result = await _protservice.ApproveAsync<int, PROTRegistro>(RegistroId);
            result.TimeString = (result.TimeValue.HasValue) ? result.TimeValue.Value.ToString(_options.Value.DateTimeFormat) : "";

            return new JsonResult(result);
        }
    }
}

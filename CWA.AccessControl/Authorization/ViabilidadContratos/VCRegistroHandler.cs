using CWA.AccessControl.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Authorization.ViabilidadContratos
{
    public class VCRegistroHandler : AuthorizationHandler<VCRegistroRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly VCAccessControlService _vccontrol;

        public VCRegistroHandler(IHttpContextAccessor Accessor, VCAccessControlService VCControl)
        {
            _accessor = Accessor;
            _vccontrol = VCControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, VCRegistroRequirement Requirement)
        {
            bool authorize = false;

            var httpcontext = _accessor.HttpContext;
            var method = httpcontext.Request.Method.ToUpper();
            var parameterRegistroid = httpcontext.GetRouteValue("registroid");
            var parameterDocumentoid = httpcontext.GetRouteValue("documentoid");
            var parameterHandler = httpcontext.GetRouteValue("handler");

            int? registroid = parameterRegistroid is not null ? int.Parse($"{parameterRegistroid}") : null;
            int? documentoid = parameterDocumentoid is not null ? int.Parse($"{parameterDocumentoid}") : null;
            string handler = parameterHandler is not null ? $"{parameterHandler}" : "";

            if (method == "GET")
            {
                authorize = (parameterDocumentoid is null && parameterHandler is null) 
                    ? await _vccontrol.AllowRegistroGetAsync(Requirement.Name, registroid)
                    : await _vccontrol.AllowDocumentoGetAsync(Requirement.Name, registroid.Value, documentoid, handler);                
            }

            if (method == "POST")
            {
                authorize = parameterRegistroid is null
                    ? await _vccontrol.AllowRegistroPostAsync(Requirement.Name)
                    : await _vccontrol.AllowRegistroPostAsync(Requirement.Name, registroid, handler);
            }

            if (authorize) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

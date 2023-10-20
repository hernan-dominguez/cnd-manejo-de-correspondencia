using CWA.AccessControl.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Authorization.GrandesClientes
{
    public class GCMedidorDistHandler : AuthorizationHandler<GCMedidorDistRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly GCAccessControlService _gccontrol;

        public GCMedidorDistHandler(IHttpContextAccessor Accessor, GCAccessControlService GCControl)
        {
            _accessor = Accessor;
            _gccontrol = GCControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, GCMedidorDistRequirement Requirement)
        {
            bool authorize = false;

            var httpcontext = _accessor.HttpContext;
            var method = httpcontext.Request.Method.ToUpper();
            var registroid = int.Parse(httpcontext.GetRouteValue("registroid").ToString());
            var medidorid = int.Parse(httpcontext.GetRouteValue("medidorid").ToString());
            var parameterHandler = httpcontext.GetRouteValue("handler");

            if (method == "GET" && parameterHandler is null) authorize = await _gccontrol.AllowMedidorDistGetAsync(registroid, medidorid);
            if (method == "POST" && parameterHandler is not null) authorize = await _gccontrol.AllowMedidorDistPostAsync(registroid, medidorid, parameterHandler.ToString());

            if (authorize) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

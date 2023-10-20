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
    public class GCComercialHandler : AuthorizationHandler<GCComercialRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly GCAccessControlService _gccontrol;

        public GCComercialHandler(IHttpContextAccessor Accessor, GCAccessControlService GCControl)
        {
            _accessor = Accessor;
            _gccontrol = GCControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, GCComercialRequirement Requirement)
        {
            bool authorize = false;

            var httpcontext = _accessor.HttpContext;
            var method = httpcontext.Request.Method.ToUpper();
            var registroid = int.Parse(httpcontext.GetRouteValue("registroid").ToString());
            var comercialid = int.Parse(httpcontext.GetRouteValue("comercialid").ToString());
            var parameterHandler = httpcontext.GetRouteValue("handler");

            if (method == "GET") authorize = parameterHandler is null && await _gccontrol.AllowComercialGetAsync(registroid, comercialid);

            if (method == "POST") authorize = parameterHandler is not null && await _gccontrol.AllowComercialPostAsync(registroid, comercialid, parameterHandler.ToString());

            if (authorize) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

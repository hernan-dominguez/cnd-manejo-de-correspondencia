using CWA.AccessControl.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Authorization.Protecciones
{
    public class PROTRegistroHandler : AuthorizationHandler<PROTRegistroRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly PROTAccessControlService _protcontrol;

        public PROTRegistroHandler(IHttpContextAccessor Accessor, PROTAccessControlService PROTControl)
        {
            _accessor = Accessor;
            _protcontrol = PROTControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, PROTRegistroRequirement Requirement)
        {
            bool authorize = false;

            var httpcontext = _accessor.HttpContext;
            var method = httpcontext.Request.Method.ToUpper();
            var registroid = int.Parse(httpcontext.GetRouteValue("registroid").ToString());
            var parameterHandler = httpcontext.GetRouteValue("handler");

            if (method == "GET") authorize = parameterHandler is null && await _protcontrol.AllowRegistroGetAsync();

            if (method == "POST") authorize = parameterHandler is not null && await _protcontrol.AllowRegistroPostAsync(registroid, parameterHandler.ToString());

            if (authorize) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

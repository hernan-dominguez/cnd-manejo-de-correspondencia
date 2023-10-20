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
    class PROTGeneralesHandler : AuthorizationHandler<PROTGeneralesRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly PROTAccessControlService _protcontrol;

        public PROTGeneralesHandler(IHttpContextAccessor Accessor, PROTAccessControlService PROTControl)
        {
            _accessor = Accessor;
            _protcontrol = PROTControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, PROTGeneralesRequirement Requirement)
        {
            bool authorize = false;

            var httpcontext = _accessor.HttpContext;
            var method = httpcontext.Request.Method.ToUpper();
            var registroid = int.Parse(httpcontext.GetRouteValue("registroid").ToString());
            var generalesid = int.Parse(httpcontext.GetRouteValue("generalesid").ToString());
            var parameterHandler = httpcontext.GetRouteValue("handler");

            if (method == "GET" && parameterHandler is null) authorize = await _protcontrol.AllowGeneralesGetAsync(registroid, generalesid);
            if (method == "POST" && parameterHandler is not null) authorize = await _protcontrol.AllowGeneralesPostAsync(registroid, generalesid, parameterHandler.ToString());

            if (authorize) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

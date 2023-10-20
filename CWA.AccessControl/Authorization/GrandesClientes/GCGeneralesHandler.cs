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
    public class GCGeneralesHandler : AuthorizationHandler<GCGeneralesRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly GCAccessControlService _gccontrol;

        public GCGeneralesHandler(IHttpContextAccessor Accessor, GCAccessControlService GCControl)
        {
            _accessor = Accessor;
            _gccontrol = GCControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, GCGeneralesRequirement Requirement)
        {
            bool authorize = false;

            var httpcontext = _accessor.HttpContext;
            var method = httpcontext.Request.Method.ToUpper();
            var registroid = int.Parse(httpcontext.GetRouteValue("registroid").ToString());
            var generalesid = int.Parse(httpcontext.GetRouteValue("generalesid").ToString());
            var parameterHandler = httpcontext.GetRouteValue("handler");

            if (method == "GET" && parameterHandler is null) authorize = await _gccontrol.AllowGeneralesGetAsync(registroid, generalesid);
            if (method == "POST" && parameterHandler is not null) authorize = await _gccontrol.AllowGeneralesPostAsync(registroid, generalesid, parameterHandler.ToString());

            if (authorize) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

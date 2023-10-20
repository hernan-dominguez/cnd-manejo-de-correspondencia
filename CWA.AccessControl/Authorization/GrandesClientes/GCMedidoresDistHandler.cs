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
    public class GCMedidoresDistHandler : AuthorizationHandler<GCMedidoresDistRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly GCAccessControlService _gccontrol;

        public GCMedidoresDistHandler(IHttpContextAccessor Accessor, GCAccessControlService GCControl)
        {
            _accessor = Accessor;
            _gccontrol = GCControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, GCMedidoresDistRequirement Requirement)
        {
            var httpcontext = _accessor.HttpContext;
            var registroid = int.Parse(httpcontext.GetRouteValue("registroid").ToString());

            if (await _gccontrol.AllowMedidoresDistGetAsync(registroid)) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

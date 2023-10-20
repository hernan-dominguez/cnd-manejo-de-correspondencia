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
    class PROTOwnershipHandler : AuthorizationHandler<PROTOwnershipRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly PROTAccessControlService _protcontrol;

        public PROTOwnershipHandler(IHttpContextAccessor Accessor, PROTAccessControlService PROTControl)
        {
            _accessor = Accessor;
            _protcontrol = PROTControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, PROTOwnershipRequirement Requirement)
        {
            var httpcontext = _accessor.HttpContext;
            int registroid = int.Parse(httpcontext.GetRouteValue("registroid").ToString());

            if (await _protcontrol.ValidRegistroOwnership(registroid)) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

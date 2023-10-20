using CWA.AccessControl.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Authorization.GrandesClientes
{
    public class GCMedicionesHandler : AuthorizationHandler<GCMedicionesRequirement>
    {
        private readonly GCAccessControlService _gccontrol;

        public GCMedicionesHandler(GCAccessControlService Control) => _gccontrol = Control;

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, GCMedicionesRequirement Requirement)
        {
            if (await _gccontrol.AllowMedicionesGetAsync()) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

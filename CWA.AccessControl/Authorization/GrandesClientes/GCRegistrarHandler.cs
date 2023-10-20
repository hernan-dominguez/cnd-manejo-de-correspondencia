using CWA.AccessControl.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Authorization.GrandesClientes
{
    public class GCRegistrarHandler : AuthorizationHandler<GCRegistrarRequirement>
    {
        private readonly GCAccessControlService _gccontrol;

        public GCRegistrarHandler(GCAccessControlService Control) => _gccontrol = Control;

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, GCRegistrarRequirement Requirement)
        {
            if (await _gccontrol.AllowRegistrarGetPostAsync()) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

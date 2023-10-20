using CWA.AccessControl.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Authorization.Protecciones
{
    public class PROTRegistrarHandler : AuthorizationHandler<PROTRegistrarRequirement>
    {
        private readonly PROTAccessControlService _protcontrol;

        public PROTRegistrarHandler(PROTAccessControlService Control) => _protcontrol = Control;

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, PROTRegistrarRequirement Requirement)
        {
            if (await _protcontrol.AllowRegistrarGetPostAsync()) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

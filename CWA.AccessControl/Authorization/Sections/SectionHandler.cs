using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Authorization.Sections
{
    public class SectionHandler : AuthorizationHandler<SectionRequirement>
    {
        public IHttpContextAccessor _accessor;
        public AppAccessControlService _access;

        public SectionHandler(IHttpContextAccessor Accessor, AppAccessControlService Access)
        {
            _accessor = Accessor;
            _access = Access;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, SectionRequirement requirement)
        {
            var httpcontext = _accessor.HttpContext;

            // Authentication required
            if (!httpcontext.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // Admin
            if (requirement.Name == SectionNames.AdminSection)
            {
                if (await _access.HasClaimAsync(ClaimNames.UserGroup, GroupNames.AdminGroup))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

            // Grandes Clientes
            if (requirement.Name == SectionNames.GCSection)
            {
                if (await _access.HasClaimAsync(ClaimNames.GCSection))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

            // Protecciones
            if (requirement.Name == SectionNames.PROTSection)
            {
                if (await _access.HasClaimAsync(ClaimNames.PROTSection))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

            // Viabilidad de contratos
            if (requirement.Name == SectionNames.VCSection)
            {
                if (await _access.HasClaimAsync(ClaimNames.VCSection))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

            return Task.CompletedTask;
        }
    }
}

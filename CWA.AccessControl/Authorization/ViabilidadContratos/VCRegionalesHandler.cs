using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Authorization.ViabilidadContratos
{
    public class VCRegionalesHandler : AuthorizationHandler<VCRegionalesRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly VCAccessControlService _vccontrol;

        public VCRegionalesHandler(IHttpContextAccessor Accessor, VCAccessControlService VCControl)
        {
            _accessor = Accessor;
            _vccontrol = VCControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, VCRegionalesRequirement Requirement)
        {
            bool authorize = false;

            var httpcontext = _accessor.HttpContext;
            var method = httpcontext.Request.Method.ToUpper();
                        
            if (method == "GET" || (method == "POST" && Requirement.Name == VCConstants.REGIONAL)) authorize = await _vccontrol.AllowRegionalesGetAsync();
            
            if (authorize) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

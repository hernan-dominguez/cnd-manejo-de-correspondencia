using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Entities.GrandesClientes;
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
    public class GCMedicionHandler : AuthorizationHandler<GCMedicionRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly GCAccessControlService _gccontrol;

        public GCMedicionHandler(IHttpContextAccessor Accessor, GCAccessControlService GCControl)
        {
            _accessor = Accessor;
            _gccontrol = GCControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, GCMedicionRequirement Requirement)
        {
            bool authorize = false;

            var httpcontext = _accessor.HttpContext;
            var method = httpcontext.Request.Method.ToUpper();
            var registroid = int.Parse(httpcontext.GetRouteValue("registroid").ToString());
            var parameterMedicionid = httpcontext.GetRouteValue("medicionid");
            var parameterHandler = httpcontext.GetRouteValue("handler");

            if (method == "GET" && parameterHandler is null)
            {
                if (parameterMedicionid is null)
                {
                    authorize = await _gccontrol.AllowMedicionGetAsync(registroid);
                }
                else
                {
                    var medicionid = int.Parse(parameterMedicionid.ToString());
                                        
                    authorize = await _gccontrol.AllowMedicionGetAsync(registroid, medicionid);
                }
            }

            if (method == "POST")
            {
                if (parameterMedicionid is null && parameterHandler is null)
                {                    
                    authorize = await _gccontrol.AllowMedicionPostAsync(registroid);
                }

                if (parameterMedicionid is not null && parameterHandler is not null)
                {
                    var medicionid = int.Parse(parameterMedicionid.ToString());
                    var hanldername = parameterHandler.ToString();

                    authorize = await _gccontrol.AllowMedicionPostAsync(registroid, medicionid, hanldername);
                }
            }

            if (authorize) Context.Succeed(Requirement);            

            return Task.CompletedTask;
        }
    }
}

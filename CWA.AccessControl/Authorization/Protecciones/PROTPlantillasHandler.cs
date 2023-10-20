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
    public class PROTPlantillasHandler : AuthorizationHandler<PROTPlantillasRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly PROTAccessControlService _protcontrol;

        public PROTPlantillasHandler(IHttpContextAccessor Accessor, PROTAccessControlService PROTControl)
        {
            _accessor = Accessor;
            _protcontrol = PROTControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, PROTPlantillasRequirement Requirement)
        {
            bool authorize = false;

            var httpcontext = _accessor.HttpContext;
            var method = httpcontext.Request.Method.ToUpper();
            //var registroid = int.Parse(httpcontext.GetRouteValue("registroid").ToString());
            var parameterPlantillaid = httpcontext.GetRouteValue("plantillaid");
            var parameterHandler = httpcontext.GetRouteValue("handler");

            if (method == "GET")
            {
                if (parameterPlantillaid is null)
                {
                    authorize = await _protcontrol.AllowPlantillasGetAsync();
                }
                else
                {
                    var plantillaid = int.Parse(parameterPlantillaid.ToString());

                    authorize = parameterHandler is not null && await _protcontrol.AllowPlantillasGetAsync(plantillaid, parameterHandler.ToString());
                }
            }

            //if (method == "POST" && parameterHandler is not null)
            //{
            //    var form = await httpcontext.Request.ReadFormAsync();
            //    int? documentoid = null;

            //    if (form.ContainsKey("EditModel.DocumentoId"))
            //    {
            //        documentoid = int.Parse(form["EditModel.DocumentoId"].ToString());
            //    }
            //    else
            //    {
            //        if (form.ContainsKey("DocumentoId")) documentoid = int.Parse(form["DocumentoId"].ToString());
            //    }

            //    authorize = await _protcontrol.AllowDocumentosPostAsync(registroid, parameterHandler.ToString(), documentoid);
            //}

            if (authorize) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

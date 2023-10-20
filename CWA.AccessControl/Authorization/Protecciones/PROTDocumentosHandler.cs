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
    public class PROTDocumentosHandler : AuthorizationHandler<PROTDocumentosRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly PROTAccessControlService _protcontrol;

        public PROTDocumentosHandler(IHttpContextAccessor Accessor, PROTAccessControlService PROTControl)
        {
            _accessor = Accessor;
            _protcontrol = PROTControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, PROTDocumentosRequirement Requirement)
        {
            bool authorize = false;

            var httpcontext = _accessor.HttpContext;
            var method = httpcontext.Request.Method.ToUpper();
            var registroid = int.Parse(httpcontext.GetRouteValue("registroid").ToString());
            var parameterDocumentoid = httpcontext.GetRouteValue("documentoid");
            var parameterHandler = httpcontext.GetRouteValue("handler");

            if (method == "GET")
            {
                if (parameterDocumentoid is null)
                {
                    authorize = await _protcontrol.AllowDocumentosGetAsync(registroid);
                }
                else
                {
                    var documentoid = int.Parse(parameterDocumentoid.ToString());

                    authorize = parameterHandler is not null && await _protcontrol.AllowDocumentosGetAsync(registroid, documentoid, parameterHandler.ToString());
                }
            }

            if (method == "POST" && parameterHandler is not null)
            {
                var form = await httpcontext.Request.ReadFormAsync();
                int? documentoid = null;

                if (form.ContainsKey("EditModel.DocumentoId"))
                {
                    documentoid = int.Parse(form["EditModel.DocumentoId"].ToString());
                }
                else
                {
                    if (form.ContainsKey("DocumentoId")) documentoid = int.Parse(form["DocumentoId"].ToString());
                }

                authorize = await _protcontrol.AllowDocumentosPostAsync(registroid, parameterHandler.ToString(), documentoid);
            }

            if (authorize) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

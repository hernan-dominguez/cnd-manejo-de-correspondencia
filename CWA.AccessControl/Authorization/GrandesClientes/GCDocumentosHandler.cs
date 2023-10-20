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
    public class GCDocumentosHandler : AuthorizationHandler<GCDocumentosRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly GCAccessControlService _gccontrol;

        public GCDocumentosHandler(IHttpContextAccessor Accessor, GCAccessControlService GCControl)
        {
            _accessor = Accessor;
            _gccontrol = GCControl;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext Context, GCDocumentosRequirement Requirement)
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
                    string handlerName = (parameterHandler is not null) ? parameterHandler.ToString() : "";

                    authorize = await _gccontrol.AllowDocumentosGetAsync(registroid, HandlerName: handlerName);
                }
                else
                {
                    var documentoid = int.Parse(parameterDocumentoid.ToString());

                    authorize = parameterHandler is not null && await _gccontrol.AllowDocumentosGetAsync(registroid, documentoid, parameterHandler.ToString());
                }
            }

            if (method == "POST" && parameterHandler is not null)
            {
                var form = await httpcontext.Request.ReadFormAsync();
                int? documentoid = null;

                if (!form.ContainsKey("EditModel.DocumentoId"))
                { 
                    if (form.ContainsKey("DocumentoId")) documentoid = int.Parse(form["DocumentoId"].ToString());
                }
                else
                {

                   documentoid = int.TryParse($"{form["EditModel.DocumentoId"]}", out int result) ? result : documentoid;
                }

                authorize = await _gccontrol.AllowDocumentosPostAsync(registroid, parameterHandler.ToString(), documentoid);
            }

            if (authorize) Context.Succeed(Requirement);

            return Task.CompletedTask;
        }
    }
}

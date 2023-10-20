using CWA.AccessControl.Constants;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Extensions
{
    public static class RazorPagesOptionsExtension
    {
        public static void AddFoldersAuthorization(this RazorPagesOptions Options)
        {
            Options.Conventions.AuthorizeFolder("/Admin", PolicyNames.AdminSectionPolicy);
            Options.Conventions.AuthorizeFolder("/GrandesClientes", PolicyNames.GCSectionPolicy);
            Options.Conventions.AuthorizeFolder("/Protecciones", PolicyNames.PROTSectionPolicy);
            Options.Conventions.AuthorizeFolder("/ViabilidadContratos", PolicyNames.VCSectionPolicy);
        }
    }
}

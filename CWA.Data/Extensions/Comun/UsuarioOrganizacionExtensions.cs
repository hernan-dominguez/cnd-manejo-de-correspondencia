using CWA.Entities.Comun;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Comun
{
    public static class UsuarioOrganizacionExtensions 
    {
        public static void Configure<T>(this ModelBuilder builder) where T : UsuarioOrganizacion
        {
            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

using CWA.Entities.Comun;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Comun
{
    public static class CatalogoExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : Catalogo
        {
            // Habilitados por default
            builder.Entity<T>().Property(p => p.Habilitado).HasDefaultValue(true);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

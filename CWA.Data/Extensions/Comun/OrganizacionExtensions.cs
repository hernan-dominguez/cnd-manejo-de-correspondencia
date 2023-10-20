using CWA.Entities.Comun;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Comun
{
    public static class OrganizacionExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : Organizacion
        {
            // Usuarios
            builder.Entity<T>()
                .HasMany(p => p.Usuarios)
                .WithOne(p => p.Organizacion as T)
                .HasForeignKey(fk => fk.OrganizacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

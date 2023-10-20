
using CWA.Entities.Comun;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Comun
{
    public static class AgenteExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : Agente
        {
            // Organizacion
            builder.Entity<T>()
                .HasOne(p => p.Organizacion)
                .WithOne(p => p.Agente as T)
                .HasForeignKey<Agente>(fk => fk.OrganizacionId);

            // Tipo agente
            builder.Entity<T>()
                .HasOne(p => p.TipoAgente)
                .WithMany()
                .HasForeignKey(fk => fk.TipoAgenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

using CWA.Entities.GrandesClientes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.GrandesClientes
{
    public static class GCGeneralesExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : GCGenerales
        {
            // Provincia
            builder.Entity<T>()
                .HasOne(p => p.Provincia)
                .WithMany()
                .HasForeignKey(fk => fk.ProvinciaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario atención
            builder.Entity<T>()
                .HasOne(p => p.UsuarioAtencion)
                .WithMany()
                .HasForeignKey(fk => fk.UsuarioAtencionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auto inclusion
            builder.Entity<T>().Navigation(n => n.Provincia).AutoInclude();

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

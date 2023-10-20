
using CWA.Entities.GrandesClientes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.GrandesClientes
{
    public static class GCDocumentoExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : GCDocumento
        {
            // Tipo documento
            builder.Entity<T>()
                .HasOne(p => p.TipoDocumento)
                .WithMany()
                .HasForeignKey(fk => fk.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario atención
            builder.Entity<T>()
                .HasOne(p => p.UsuarioAtencion)
                .WithMany()
                .HasForeignKey(fk => fk.UsuarioAtencionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

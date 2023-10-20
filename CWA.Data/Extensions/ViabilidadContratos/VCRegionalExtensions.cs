using CWA.Entities.ViabilidadContratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.ViabilidadContratos
{
    public static class VCRegionalExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : VCRegional
        {
            // Tipo de solicitud
            builder.Entity<T>()
                .HasOne(p => p.TipoSolicitud)
                .WithMany()
                .HasForeignKey(fk => fk.TipoSolicitudId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tipo de transacción
            builder.Entity<T>()
                .HasOne(p => p.TipoTransaccion)
                .WithMany()
                .HasForeignKey(fk => fk.TipoTransaccionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Solicitante
            builder.Entity<T>()
                .HasOne(p => p.Solicitante)
                .WithMany()
                .HasForeignKey(fk => fk.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            // País
            builder.Entity<T>()
                .HasOne(p => p.Pais)
                .WithMany()
                .HasForeignKey(fk => fk.PaisId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contraparte
            builder.Entity<T>()
                .HasOne(p => p.Contraparte)
                .WithMany()
                .HasForeignKey(fk => fk.ContraparteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario aprobación
            builder.Entity<T>()
                .HasOne(p => p.UsuarioAtencion)
                .WithMany()
                .HasForeignKey(fk => fk.UsuarioAtencionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Documentos
            builder.Entity<T>()
                .HasMany(p => p.Documentos)
                .WithOne(p => p.Registro as T)
                .HasForeignKey(fk => fk.RegistroId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auto inclusion
            builder.Entity<T>().Navigation(n => n.TipoSolicitud).AutoInclude();
            builder.Entity<T>().Navigation(n => n.TipoTransaccion).AutoInclude();
            builder.Entity<T>().Navigation(n => n.Solicitante).AutoInclude();
            builder.Entity<T>().Navigation(n => n.Contraparte).AutoInclude();

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

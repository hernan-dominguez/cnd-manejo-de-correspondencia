using CWA.Entities.ViabilidadContratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.ViabilidadContratos
{
    public static class VCEnmiendaExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : VCEnmienda
        {
            // Registro
            builder.Entity<T>()
                .HasOne(p => p.Contrato)
                .WithMany()
                .HasForeignKey(fk => fk.ContratoId)
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
            builder.Entity<T>().Navigation(n => n.Contrato).AutoInclude();

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

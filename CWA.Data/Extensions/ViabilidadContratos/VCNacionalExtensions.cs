using CWA.Entities.ViabilidadContratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.ViabilidadContratos
{
    public static class VCNacionalExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : VCNacional
        {
            // Código
            // builder.Entity<T>()
            //     .Property(p => p.CodigoDisplay)
            //     .HasComputedColumnSql("CODIGO || '-' TO_CHAR(ID)");

            // Tipo de contrato
            builder.Entity<T>()
                .HasOne(p => p.TipoContrato)
                .WithMany()
                .HasForeignKey(fk => fk.TipoContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vendedor
            builder.Entity<T>()
                .HasOne(p => p.Vendedor)
                .WithMany()
                .HasForeignKey(fk => fk.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Comprador
            builder.Entity<T>()
                .HasOne(p => p.Comprador)
                .WithMany()
                .HasForeignKey(fk => fk.CompradorId)
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
            builder.Entity<T>().Navigation(n => n.TipoContrato).AutoInclude();
            builder.Entity<T>().Navigation(n => n.Vendedor).AutoInclude();
            builder.Entity<T>().Navigation(n => n.Comprador).AutoInclude();

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

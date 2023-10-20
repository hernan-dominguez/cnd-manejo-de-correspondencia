using CWA.Entities.GrandesClientes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.GrandesClientes
{
    public static class GCMedicionSmecExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : GCMedicionSmec
        {
            // Tensión
            builder.Entity<T>()
                .HasOne(p => p.Tension)
                .WithMany()
                .HasForeignKey(fk => fk.TensionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Distribuidora
            builder.Entity<T>()
                .HasOne(p => p.Distribuidora)
                .WithMany()
                .HasForeignKey(fk => fk.DistribuidoraId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tipo de identificación distribuidora
            builder.Entity<T>()
                .HasOne(p => p.TipoDistIdentificacion)
                .WithMany()
                .HasForeignKey(fk => fk.TipoDistIdentificacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Subestación
            builder.Entity<T>()
                .HasOne(p => p.Subestacion)
                .WithMany()
                .HasForeignKey(fk => fk.SubestacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Línea
            builder.Entity<T>()
                .HasOne(p => p.Linea)
                .WithMany()
                .HasForeignKey(fk => fk.LineaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Certificado
            builder.Entity<T>()
                .HasOne(p => p.SAS)
                .WithMany()
                .HasForeignKey(fk => fk.DocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario atención
            builder.Entity<T>()
                .HasOne(p => p.UsuarioAtencion)
                .WithMany()
                .HasForeignKey(fk => fk.UsuarioAtencionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auto inclusion
            builder.Entity<T>().Navigation(n => n.Registro).AutoInclude();
            builder.Entity<T>().Navigation(n => n.Distribuidora).AutoInclude();
            builder.Entity<T>().Navigation(n => n.TipoDistIdentificacion).AutoInclude();
            builder.Entity<T>().Navigation(n => n.Linea).AutoInclude();
            builder.Entity<T>().Navigation(n => n.Tension).AutoInclude();
            builder.Entity<T>().Navigation(n => n.Subestacion).AutoInclude();

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

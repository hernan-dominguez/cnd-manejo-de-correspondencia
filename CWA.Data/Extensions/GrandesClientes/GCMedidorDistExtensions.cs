using CWA.Entities.GrandesClientes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.GrandesClientes
{
    public static class GCMedidorDistExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : GCMedidorDist
        {
            // Fabricante
            builder.Entity<T>()
                .HasOne(p => p.Fabricante)
                .WithMany()
                .HasForeignKey(fk => fk.FabricanteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Modelo
            builder.Entity<T>()
                .HasOne(p => p.Modelo)
                .WithMany()
                .HasForeignKey(fk => fk.ModeloId)
                .OnDelete(DeleteBehavior.Restrict);

            // Canales
            builder.Entity<T>()
                .HasMany(p => p.Canales)
                .WithOne(p => p.Medidor as T)
                .HasForeignKey(fk => fk.MedidorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario atención
            builder.Entity<T>()
                .HasOne(p => p.UsuarioAtencion)
                .WithMany()
                .HasForeignKey(fk => fk.UsuarioAtencionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auto inclusion
            builder.Entity<T>().Navigation(n => n.Canales).AutoInclude();
            builder.Entity<T>().Navigation(n => n.Fabricante).AutoInclude();
            builder.Entity<T>().Navigation(n => n.Modelo).AutoInclude();

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

using CWA.Entities.GrandesClientes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.GrandesClientes
{
    public static class GCMedidorCanalExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : GCMedidorCanal
        {
            // Primary key
            builder.Entity<T>().HasKey(p => new { p.MedidorId, p.Numero });

            // Canal descripción
            builder.Entity<T>()
                .HasOne(p => p.Descripcion)
                .WithMany()
                .HasForeignKey(fk => fk.DescripcionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auto inclusion
            builder.Entity<T>().Navigation(n => n.Descripcion).AutoInclude();
        }
    }
}
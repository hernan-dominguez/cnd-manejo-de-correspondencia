using CWA.Entities.Comun;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Comun
{
    public static class ResponsableGranClienteExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : ResponsableGranCliente
        {
            // Primary key
            builder.Entity<T>().HasKey(p => p.GranClienteId);

            // Responsable
            builder.Entity<T>()
                .HasOne(p => p.Responsable)
                .WithMany()
                .HasForeignKey(fk => fk.ResponsableId)
                .OnDelete(DeleteBehavior.Restrict);

            // Gran Cliente
            builder.Entity<T>()
                .HasOne(p => p.GranCliente)
                .WithMany()
                .HasForeignKey(fk => fk.GranClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

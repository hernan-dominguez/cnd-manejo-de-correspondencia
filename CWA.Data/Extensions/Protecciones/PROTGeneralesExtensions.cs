using CWA.Entities.Protecciones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Protecciones
{
    public static class PROTGeneralesExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : PROTGenerales
        {
            // Provincia
            builder.Entity<T>()
                .HasOne(p => p.Provincia)
                .WithMany()
                .HasForeignKey(fk => fk.ProvinciaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auto inclusion
            builder.Entity<T>().Navigation(n => n.Provincia).AutoInclude();

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

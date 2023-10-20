using CWA.Entities.Comun;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Comun
{
    public static class AgenteRegionalExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : AgenteRegional
        {
            // País
            builder.Entity<T>()
                .HasOne(p => p.Pais)
                .WithMany()
                .HasForeignKey(fk => fk.PaisId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

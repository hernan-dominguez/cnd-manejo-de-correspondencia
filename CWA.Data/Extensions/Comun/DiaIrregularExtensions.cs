using CWA.Entities.Comun;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Comun
{
    public static class DiaIrregularExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : DiaIrregular
        {
            // Condiciones
            builder.Entity<T>()
                .HasOne(p => p.Condicion)
                .WithMany()
                .HasForeignKey(fk => fk.CondicionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

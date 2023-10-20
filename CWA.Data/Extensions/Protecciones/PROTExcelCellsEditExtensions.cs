using CWA.Entities.Protecciones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Protecciones
{
    public static class PROTExcelCellsEditExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : PROTExcelCellsEdit
        {
            // Registro Plantilla
            builder.Entity<T>()
                .HasOne(p => p.RegPlantilla)
                .WithMany()
                .HasForeignKey(fk => fk.RegPlantillaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

using CWA.Entities.ViabilidadContratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.ViabilidadContratos
{
    public static class VCDocNacionalExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : VCDocNacional
        {
            // Tipo Documento
            builder.Entity<T>()
                .HasOne(p => p.TipoDocumento)
                .WithMany()
                .HasForeignKey(fk => fk.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

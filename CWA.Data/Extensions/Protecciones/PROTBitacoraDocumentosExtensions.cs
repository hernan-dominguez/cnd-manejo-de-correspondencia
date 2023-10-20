using CWA.Entities.Protecciones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Protecciones
{
    public static class PROTBitacoraDocumentosExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : PROTBitacoraDocumentos
        {
            // Tipo documento
            builder.Entity<T>()
                .HasOne(p => p.TipoDocumento)
                .WithMany()
                .HasForeignKey(fk => fk.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using CWA.Entities.ViabilidadContratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.GrandesClientes
{
    public static class VCDestinatarioExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : VCDestinatario
        {
            // Usuario
            builder.Entity<T>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(fk => fk.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Grupo
            builder.Entity<T>()
                .HasOne(p => p.Notificacion)
                .WithMany()
                .HasForeignKey(fk => fk.NotificacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

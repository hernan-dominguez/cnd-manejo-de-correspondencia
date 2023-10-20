using CWA.Entities.Protecciones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Protecciones
{
    public static class PROTRegistroExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : PROTRegistro
        {
            // Tipo Gran Cliente
            builder.Entity<T>()
                .HasOne(p => p.Tipo)
                .WithMany()
                .HasForeignKey(fk => fk.TipoAgenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario habilita
            builder.Entity<T>()
                .HasOne(p => p.UsuarioAprueba)
                .WithMany()
                .HasForeignKey(fk => fk.UsuarioApruebaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Generales
            builder.Entity<T>()
                .HasOne(p => p.Generales)
                .WithMany()
                .HasForeignKey(fk => fk.GeneralesId)
                .OnDelete(DeleteBehavior.Restrict);

            // Documentos
            builder.Entity<T>()
                .HasMany(p => p.Documentos)
                .WithOne(p => p.Registro as T)
                .HasForeignKey(fk => fk.RegistroId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

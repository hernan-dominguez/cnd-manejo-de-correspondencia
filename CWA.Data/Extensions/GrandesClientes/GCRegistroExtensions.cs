using CWA.Entities.GrandesClientes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.GrandesClientes
{
    public static class GCRegistroExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : GCRegistro
        {
            // Responsable
            builder.Entity<T>()
                .HasOne(p => p.Responsable)
                .WithMany()
                .HasForeignKey(fk => fk.ResponsableId)
                .OnDelete(DeleteBehavior.Restrict);

            // Propietario
            builder.Entity<T>()
                .HasOne(p => p.Propietario)
                .WithMany()
                .HasForeignKey(fk => fk.PropietarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tipo Gran Cliente
            builder.Entity<T>()
                .HasOne(p => p.TipoGranCliente)
                .WithMany()
                .HasForeignKey(fk => fk.TipoGranClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Usuario atención
            builder.Entity<T>()
                .HasOne(p => p.UsuarioAtencion)
                .WithMany()
                .HasForeignKey(fk => fk.UsuarioAtencionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Generales
            builder.Entity<T>()
                .HasOne(p => p.Generales)
                .WithMany()
                .HasForeignKey(fk => fk.GeneralesId)
                .OnDelete(DeleteBehavior.Restrict);

            // Comercial
            builder.Entity<T>()
                .HasOne(p => p.Comercial)
                .WithMany()
                .HasForeignKey(fk => fk.ComercialId)
                .OnDelete(DeleteBehavior.Restrict);

            // Mediciones
            builder.Entity<T>()
                .HasMany(p => p.Mediciones)
                .WithOne(p => p.Registro as T)
                .HasForeignKey(fk => fk.RegistroId)
                .OnDelete(DeleteBehavior.Restrict);

            // Documentos
            builder.Entity<T>()
                .HasMany(p => p.Documentos)
                .WithOne(p => p.Registro as T)
                .HasForeignKey(fk => fk.RegistroId)
                .OnDelete(DeleteBehavior.Restrict);

            // Estatus
            builder.Entity<T>()
                .HasOne(p => p.Estatus)
                .WithMany()
                .HasForeignKey(fk => fk.EstatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
        }
    }
}

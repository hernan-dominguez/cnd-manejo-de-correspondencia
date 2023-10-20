using CWA.Entities.Comun;
using CWA.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions.Identity
{
    public static class IdentiyExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : AppUser
        {
            builder.RenameTables<T>();

            // Entidad
            builder.Entity<T>()
                .HasOne(p => p.Organizacion)
                .WithOne(p => p.Usuario as T)
                .HasForeignKey<UsuarioOrganizacion>(fk => fk.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Claims
            builder.Entity<T>()
                .HasMany(p => p.Claims)
                .WithOne()
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.SetAuditingDateSQL<T>();
            builder.SetAuditingDateSQL<AppUserClaim>();
            builder.SetAuditingDateSQL<AppUserLogin>();
        }

        private static void RenameTables<T>(this ModelBuilder builder) where T : AppUser
        {
            // Application tables
            builder.Entity<T>().ToTable("APPUSER");
            builder.Entity<AppUserClaim>().ToTable("APPUSERCLAIM");
            builder.Entity<AppUserLogin>().ToTable("APPUSERLOGIN");

            // Unused tables
            builder.Entity<IdentityUserToken<int>>().ToTable("X_USERTOKEN_NOTUSED");
            builder.Entity<IdentityRole<int>>().ToTable("X_ROLE_NOTUSED");
            builder.Entity<IdentityUserRole<int>>().ToTable("X_USERROLE_NOTUSED");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("X_ROLECLAIM_NOTUSED");
        }
    }
}

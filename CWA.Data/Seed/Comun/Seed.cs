using CWA.Entities.Comun;
using CWA.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CWA.Data.Seed.Comun
{
    public class Seed
    {
        private static readonly string AdminUsername = "APPADM";

        private static readonly string CndOrganizacion = "Centro Nacional de Despacho";

        public static async Task<int> ExecuteAsync(UserManager<AppUser> Manager, DataContext Context)
        {
            int AdminId = await AddAdminAsync(Manager, Context);

            await AddCatalogoAsync(Context, AdminId);
            await AddDiasIrregularesAsync(Context, AdminId);
            await AddAgenteRegionalAsync(Context, AdminId);

            int CndId = await AddCndOrganizacionAsync(Context, AdminId);

            await AddUsuarioCndAsync(Manager, Context, AdminId, CndId);
            await AddOrganizacionAgenteAsync(Context, AdminId);
            // await AddOrganizacionGranCliente(Context, AdminId); DO NOT USE
            await AddUsuarioAgenteAsync(Manager, Context, AdminId, CndId);
            await AddOrganizacionNoAgenteAsync(Context, AdminId, CndId);
            await AddUsuarioNoAgenteAsync(Manager, Context, AdminId, CndId);

            return AdminId;
        }

        private static async Task<int> AddAdminAsync(UserManager<AppUser> userManager, DataContext context)
        {
            var adminUser = await context.Users.Where(w => w.NormalizedUserName == AdminUsername).FirstOrDefaultAsync();

            if (!(adminUser is null)) return adminUser.Id;

            // Create admin user
            var admin = new AppUser
            {
                Nombre = "Application Administrator",
                UserName = AdminUsername,
                Email = $"{AdminUsername.ToLower()}@cndwebapps",
                RegUsuarioId = 0,
                ModUsuarioId = 0
            };

            await userManager.CreateAsync(admin, $"*dgs{DateTime.Now.Year}*");

            // Update registration with auto-generated id
            admin.RegUsuarioId = admin.Id;
            admin.ModUsuarioId = admin.Id;

            context.Update(admin);
            await context.SaveChangesAsync();

            return admin.Id;
        }

        private static async Task AddCatalogoAsync(DataContext context, int AdminId)
        {
            if (await context.Catalogos.AnyAsync()) return;

            var jsonData = await File.ReadAllTextAsync("../SeedData/Catalogo.json");
            var catalogos = JsonSerializer.Deserialize<List<Catalogo>>(jsonData);

            foreach (var item in catalogos)
            {
                Catalogo catalogo = new()
                {
                    Id = item.Id,
                    Grupo = item.Grupo,
                    Descripcion = item.Descripcion,
                    RefVal1 = item.RefVal1,
                    RefVal2 = item.RefVal2,
                    RefVal3 = item.RefVal3,
                    RefVal4 = item.RefVal4,
                    RefVal5 = item.RefVal5,
                    RegUsuarioId = AdminId,
                    ModUsuarioId = AdminId
                };

                // 'Habilitado' property defaults to true

                context.Catalogos.Add(catalogo);
            }

            await context.SaveChangesAsync();
        }

        private static async Task AddDiasIrregularesAsync(DataContext context, int AdminId)
        {
            if (await context.DiasIrregulares.AnyAsync()) return;

            var jsonData = await File.ReadAllTextAsync("../SeedData/Dias.json", Encoding.UTF8);
            var dias = JsonSerializer.Deserialize<List<DiaIrregular>>(jsonData);

            foreach (var item in dias)
            {
                DiaIrregular dia = new()
                {
                    Dia = item.Dia,
                    CondicionId = item.CondicionId,
                    Descripcion = item.Descripcion,
                    RegUsuarioId = AdminId,
                    ModUsuarioId = AdminId
                };

                context.DiasIrregulares.Add(dia);
            }

            await context.SaveChangesAsync();
        }

        private static async Task AddAgenteRegionalAsync(DataContext context, int AdminId)
        {
            if (await context.AgentesRegionales.AnyAsync()) return;

            var jsonData = await File.ReadAllTextAsync("../SeedData/AgenteRegional.json");
            var regionales = JsonSerializer.Deserialize<List<AgenteRegional>>(jsonData);

            foreach (var item in regionales) item.RegUsuarioId = AdminId = item.ModUsuarioId = AdminId;

            context.AddRange(regionales);
            await context.SaveChangesAsync();
        }

        private static async Task<int> AddCndOrganizacionAsync(DataContext context, int AdminId)
        {
            var cnd = await context.Organizaciones.Where(w => w.Nombre.ToUpper() == CndOrganizacion.ToUpper()).FirstOrDefaultAsync();

            if (cnd is not null) return cnd.Id;

            // Create organizacion Cnd
            var org = new Organizacion { Nombre = CndOrganizacion, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
            context.Add(org);
            await context.SaveChangesAsync();

            // Add to admin
            context.Add(new UsuarioOrganizacion { UsuarioId = AdminId, OrganizacionId = org.Id, RegUsuarioId = AdminId, ModUsuarioId = AdminId });
            await context.SaveChangesAsync();

            return org.Id;
        }

        private static async Task AddUsuarioCndAsync(UserManager<AppUser> userManager, DataContext context, int AdminId, int CndId)
        {
            if (await context.UsuariosOrganizaciones.Where(w => w.OrganizacionId == CndId && w.UsuarioId != AdminId).AnyAsync()) return;

            var jsonData = await File.ReadAllTextAsync("../SeedData/UsuariosCnd.json");
            var usuarios = JsonSerializer.Deserialize<List<UsuarioCnd>>(jsonData);

            foreach (var item in usuarios)
            {
                // AppUser
                var user = new AppUser { Nombre = item.Nombre, Email = item.Email, UserName = item.UserName, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                await userManager.CreateAsync(user, $"*cnd{DateTime.Now.Year}");

                // Login provider (Microsoft)
                var login = new AppUserLogin { LoginProvider = "Microsoft", ProviderKey = item.LoginId, ProviderDisplayName = "Microsoft", UserId = user.Id, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                context.UserLogins.Add(login);

                // Full name claim
                var fullname = new AppUserClaim { ClaimType = "FULL-NAME", ClaimValue = item.Nombre, UserId = user.Id, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                context.UserClaims.Add(fullname);

                // Group claim
                var group = new AppUserClaim { ClaimType = "USER-GROUP", ClaimValue = item.Group, UserId = user.Id, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                context.UserClaims.Add(group);

                // Organizacion
                user.Organizacion = new UsuarioOrganizacion { UsuarioId = user.Id, OrganizacionId = CndId, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                context.Update(user);

                // Finalize
                await context.SaveChangesAsync();
            }
        }

        private static async Task AddOrganizacionAgenteAsync(DataContext context, int AdminId)
        {
            if (await context.Agentes.AnyAsync()) return;

            var jsonData = await File.ReadAllTextAsync("../SeedData/OrganizacionesAgentes.json");
            var organizaciones = JsonSerializer.Deserialize<List<OrganizacionAgente>>(jsonData);

            foreach (var item in organizaciones)
            {
                // Add Organizacion
                var organizacion = new Organizacion
                {
                    Nombre = item.Nombre,
                    ContactoNombre = item.Nombre,
                    ContactoTelefono = "300-9000",
                    ContactoCorreo = item.TipoAgenteId == "TAG01" ? "{correo_generador}" : "{correo_otros}", // Add external email addresses for testing
                    RegUsuarioId = AdminId,
                    ModUsuarioId = AdminId
                };

                context.Add(organizacion);
                await context.SaveChangesAsync();

                // Add Agente
                var agente = new Agente
                {
                    OrganizacionId = organizacion.Id,
                    IdBdi = item.IdBdi,
                    Codigo = item.Codigo,
                    TipoAgenteId = item.TipoAgenteId,
                    RegUsuarioId = AdminId,
                    ModUsuarioId = AdminId
                };

                context.Add(agente);
                await context.SaveChangesAsync();
            }
        }

        public static async Task AddOrganizacionGranCliente(DataContext context, int AdminId)
        {
            if (await context.Agentes.Where(w => w.TipoAgenteId == "TAG06").AnyAsync()) return;

            var jsonData = await File.ReadAllTextAsync("../SeedData/OrganizacionesGrandesClientes.json");
            var organizaciones = JsonSerializer.Deserialize<List<OrganizacionAgente>>(jsonData);

            foreach (var item in organizaciones)
            {
                // Check for parent organization
                var parent = await context.Agentes.Where(w => w.IdBdi == item.IdBdiResp).FirstOrDefaultAsync();

                if (parent is null) continue;

                // Add Organizacion
                var organizacion = new Organizacion
                {
                    Nombre = item.Nombre,
                    ContactoNombre = item.Nombre,
                    ContactoTelefono = "300-9000",
                    ContactoCorreo = "{correo_organizacion}", // Add external mail address for testing
                    RegUsuarioId = AdminId,
                    ModUsuarioId = AdminId
                };

                context.Add(organizacion);
                await context.SaveChangesAsync();

                // Add Agente
                var agente = new Agente
                {
                    OrganizacionId = organizacion.Id,
                    IdBdi = item.IdBdi,
                    Codigo = item.Codigo,
                    TipoAgenteId = item.TipoAgenteId,
                    RegUsuarioId = AdminId,
                    ModUsuarioId = AdminId
                };

                context.Add(agente);
                await context.SaveChangesAsync();

                // Add parent
                context.Add(new ResponsableGranCliente
                {
                    GranClienteId = agente.Id,
                    ResponsableId = parent.Id,
                    RegUsuarioId = AdminId,
                    ModUsuarioId = AdminId
                });

                await context.SaveChangesAsync();
            }
        }
        
        private static async Task AddUsuarioAgenteAsync(UserManager<AppUser> userManager, DataContext context, int AdminId, int CndId)
        {
            if (await context.UsuariosOrganizaciones.Where(w => w.OrganizacionId != CndId && w.Organizacion.Agente != null).AnyAsync()) return;

            var jsonData = await File.ReadAllTextAsync("../SeedData/UsuariosAgentes.json");
            var usuarios = JsonSerializer.Deserialize<List<UsuarioAgente>>(jsonData);

            foreach (var item in usuarios)
            {
                // AppUser
                var user = new AppUser { Nombre = item.Nombre, Email = item.Email, UserName = item.UserName, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                await userManager.CreateAsync(user, $"*cnd{DateTime.Now.Year}");

                // Full name claim
                var fullname = new AppUserClaim { ClaimType = "FULL-NAME", ClaimValue = item.Nombre, UserId = user.Id, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                context.UserClaims.Add(fullname);

                // Group claim
                var group = new AppUserClaim { ClaimType = "USER-GROUP", ClaimValue = item.Group, UserId = user.Id, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                context.UserClaims.Add(group);

                // Modules claims
                AddModuleClaims(context, user.Id, AdminId, new List<string> { "GC-SECTION", "GC-VIEW", "GC-UPDATE" });
                AddModuleClaims(context, user.Id, AdminId, new List<string> { "VC-SECTION", "VC-VIEW", "VC-UPDATE" });

                // Organizacion
                var agente = await context.Agentes.Where(w => w.IdBdi == item.IdBdi).FirstOrDefaultAsync();

                if (agente is not null)
                {
                    user.Organizacion = new UsuarioOrganizacion { UsuarioId = user.Id, OrganizacionId = agente.OrganizacionId, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                    context.Update(user);
                }

                // Finalize
                await context.SaveChangesAsync();
            }
        }        

        private static async Task AddOrganizacionNoAgenteAsync(DataContext context, int AdminId, int CndId)
        {
            if (await context.Organizaciones.Where(w => w.Agente == null && w.Id != CndId).AnyAsync()) return;

            var jsonData = await File.ReadAllTextAsync("../SeedData/OrganizacionesNoAgentes.json");
            var organizaciones = JsonSerializer.Deserialize<List<OrganizacionNoAgente>>(jsonData);

            foreach (var item in organizaciones)
            {
                // Add Organizacion
                var organizacion = new Organizacion
                {
                    Nombre = item.Nombre,
                    RegUsuarioId = AdminId,
                    ModUsuarioId = AdminId
                };

                context.Add(organizacion);              
            }


            await context.SaveChangesAsync();
        }

        private static async Task AddUsuarioNoAgenteAsync(UserManager<AppUser> userManager, DataContext context, int AdminId, int CndId)
        {
            if (await context.UsuariosOrganizaciones.Where(w => w.OrganizacionId != CndId && w.Organizacion.Agente == null).AnyAsync()) return;

            var jsonData = await File.ReadAllTextAsync("../SeedData/UsuariosNoAgentes.json");
            var usuarios = JsonSerializer.Deserialize<List<UsuarioNoAgente>>(jsonData);

            foreach (var item in usuarios)
            {
                // AppUser
                var user = new AppUser { Nombre = item.Nombre, Email = item.Email, UserName = item.UserName, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                await userManager.CreateAsync(user, $"*cnd{DateTime.Now.Year}");

                // Full name claim
                var fullname = new AppUserClaim { ClaimType = "FULL-NAME", ClaimValue = item.Nombre, UserId = user.Id, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                context.UserClaims.Add(fullname);

                // Group claim
                var group = new AppUserClaim { ClaimType = "USER-GROUP", ClaimValue = item.Group, UserId = user.Id, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                context.UserClaims.Add(group);

                // Organizacion
                var org = await context.Organizaciones.Where(w => w.Nombre.ToUpper() == item.Org.ToUpper()).FirstOrDefaultAsync();

                if (org is not null)
                {
                    user.Organizacion = new UsuarioOrganizacion { UsuarioId = user.Id, OrganizacionId = org.Id, RegUsuarioId = AdminId, ModUsuarioId = AdminId };
                    context.Update(user);
                }

                // Finalize
                await context.SaveChangesAsync();
            }
        }

        private static void AddModuleClaims(DataContext context, int UserId, int AdminId, List<string> ClaimTypes)
        {
            foreach (string claim in ClaimTypes)
            {
                context.UserClaims.Add(new AppUserClaim 
                { 
                    ClaimType = claim, 
                    ClaimValue = "1", 
                    UserId = UserId, 
                    RegUsuarioId = AdminId, 
                    ModUsuarioId = AdminId 
                });
            }
        }
    }
}

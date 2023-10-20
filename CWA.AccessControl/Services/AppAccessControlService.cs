using CWA.AccessControl.Constants;
using CWA.Data;
using CWA.Entities.Identity;
using CWA.Models.Comun;
using CWA.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Services
{
    public class AppAccessControlService
    {
        private readonly DataContext _data;
        private readonly UserManager<AppUser> _usermanager;
        private readonly IHttpContextAccessor _accessor;

        public int SessionUserId
        {
            get
            {
                var httpcontext = _accessor.HttpContext;
                return int.Parse(_usermanager.GetUserId(httpcontext.User));
            }
        }

        public AppAccessControlService(DataContext Data, UserManager<AppUser> Manager, IHttpContextAccessor Accesor)
        {
            _data = Data;
            _usermanager = Manager;
            _accessor = Accesor;
        }

        public async Task<bool> HasClaimAsync(string ClaimName, string ClaimValue = "", int? UserId = null)
        {
            bool hasclaim = false;            
            var httpcontext = _accessor.HttpContext;
                        
            if (!UserId.HasValue)
            {
                hasclaim = (ClaimValue.Empty())
                    ? httpcontext.User.HasClaim(c => c.Type == ClaimName)
                    : httpcontext.User.HasClaim(c => c.Type == ClaimName && c.Value == ClaimValue);
            }
            else
            {
                hasclaim = (ClaimValue.Empty())
                ? await _data.UserClaims.Where(w => w.UserId == UserId && w.ClaimType == ClaimName).AnyAsync()
                : await _data.UserClaims.Where(w => w.UserId == UserId && w.ClaimType == ClaimName && w.ClaimValue == ClaimValue).AnyAsync();                                
            }           

            return hasclaim;
        }

        public async Task<bool> IsInGroup(string[] Groups, int? UserId = null)
        {
            bool ingroup = false;
            var httpcontext = _accessor.HttpContext;

            if (!UserId.HasValue)
            {
                ingroup = httpcontext.User.HasClaim(c => c.Type == ClaimNames.UserGroup && Groups.Contains(c.Value));
            }
            else
            {
                ingroup = await _data.UserClaims.Where(w => w.UserId == UserId && w.ClaimType == ClaimNames.UserGroup && Groups.Contains(w.ClaimValue)).AnyAsync();
            }

            return ingroup;
        }

        public async Task<string> GetGroupAsync(int? UserId = null)
        {
            var httpcontext = _accessor.HttpContext;

            if (!UserId.HasValue)
            {
                var claim = httpcontext.User.Claims.Where(w => w.Type == ClaimNames.UserGroup).FirstOrDefault();
                return claim is null ? "" : claim.Value;
            }
            else
            {
                var claim = await _data.UserClaims.Where(w => w.UserId == UserId && w.ClaimType == ClaimNames.UserGroup).FirstOrDefaultAsync();
                return claim is null ? "" : claim.ClaimValue;
            }
        }

        public async Task<UserOrganization> GetOrganizacion(int? UserId = null)
        {
            UserId ??= this.SessionUserId;

            var lookup = await _data.UsuariosOrganizaciones
                .Where(w => w.UsuarioId == UserId)
                .Include(i => i.Organizacion)
                .ThenInclude(t => t.Agente)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (lookup is null) return new();

            return new UserOrganization
            {
                Id = lookup.Organizacion is not null ? lookup.Organizacion.Id : null,
                Nombre = lookup.Organizacion is not null ? lookup.Organizacion.Nombre : "",
                AgenteId = lookup.Organizacion is not null && lookup.Organizacion.Agente is not null ? lookup.Organizacion.Agente.Id : null,
                IdBdi = lookup.Organizacion is not null && lookup.Organizacion.Agente is not null ? lookup.Organizacion.Agente.IdBdi : "",
                TipoAgenteId = lookup.Organizacion is not null && lookup.Organizacion.Agente is not null ? lookup.Organizacion.Agente.TipoAgenteId : null,
            };
        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using CWA.AccessControl;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using CWA.Entities.Identity;
using CWA.Models.Comun;
using CWA.Shared.Helpers;
using CWA.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CWA.Shared.Extensions;
using Microsoft.Extensions.Logging;

namespace CWA.Application.Services.Identity
{
    public class AppUserService : BaseService
    {
        public AppUserService(DataContext Context, AppAccessControlService Access, IMapper Mapper, ILogger<BaseService> Logger) : base(Context, Access, Mapper, Logger) { }

        public async Task<List<AppUserListing>> GetAppUserListingAsync()
        {
            var query = (await _data.Users.OrderBy(o => o.Nombre).ToListAsync()).AsQueryable();

            return query.ProjectTo<AppUserListing>(_mapper.ConfigurationProvider).AsNoTracking().ToList();
        }

        public async Task<List<SelectListItem>> GetForSelectByClaim(string ClaimName, string[] Grupos = null, bool? EnOrganizacion = null)
        {
            // Lookup Section
            var lookup = await _data.UserClaims.Where(w => w.ClaimType == ClaimName).Select(i => i.UserId).ToListAsync();
            if (lookup is null) return new();

            // Lookup Group
            lookup = await _data.UserClaims.Where(w => w.ClaimType == ClaimNames.UserGroup && (Grupos == null || Grupos.Contains(w.ClaimValue))).Select(i => i.UserId).ToListAsync();
            if (lookup is null) return new();

            // Get users
            var query = _data.Users.Where(w => lookup.Contains(w.Id) && !w.Locked);

            // Filter
            if (EnOrganizacion.HasValue)
            {
                query = EnOrganizacion.Value ? query.Where(w => w.Organizacion != null) : query.Where(w => w.Organizacion == null);
            }            

            return await query.ProjectTo<SelectListItem>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }
    }
}

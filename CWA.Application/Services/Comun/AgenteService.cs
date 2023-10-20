using AutoMapper;
using AutoMapper.QueryableExtensions;
using CWA.AccessControl;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using CWA.Entities.Identity;
using CWA.Models.Comun;
using CWA.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CWA.Application.Services.Comun
{
    public class AgenteService : BaseService
    {
        public AgenteService(DataContext Context, AppAccessControlService Access, IMapper Mapper, ILogger<BaseService> Logger) : base(Context, Access, Mapper, Logger) { }

        public async Task<AgenteListing> GetByUserAsync(int? UserId = null)
        {
            UserId ??= _access.SessionUserId;

            var lookup = await _data.UsuariosOrganizaciones.Where(w => w.UsuarioId == UserId && w.Organizacion != null && w.Organizacion.Agente != null)
                .Include(i => i.Organizacion)
                .ThenInclude(t => t.Agente)
                .FirstOrDefaultAsync();

            if (lookup is null) return new();

            return _mapper.Map<AgenteListing>(lookup.Organizacion.Agente);
        }

        public async Task<List<TModel>> GetGrandesClientesByUserAsync<TModel>(int? UserId = null) where TModel : class
        {
            UserId ??= _access.SessionUserId;

            var lookup = await _data.UsuariosOrganizaciones.Where(w => w.UsuarioId == UserId && w.Organizacion != null && w.Organizacion.Agente != null).FirstOrDefaultAsync();

            if (lookup is null) return new();

            var query = _data.ResponsablesGrandesClientes.Where(w => w.ResponsableId == lookup.Organizacion.Agente.Id).Select(s => s.GranCliente).AsQueryable();

            return await query.ProjectTo<TModel>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<List<ResponsableGranClienteListing>> GetResponsablesGrandesClientesAsync()
        {
            var query = _data.ResponsablesGrandesClientes
                .Include(i => i.GranCliente)
                .ThenInclude(t => t.Organizacion)
                .Include(i => i.Responsable)
                .ThenInclude(t => t.Organizacion)
                .OrderBy(o => o.Responsable.Organizacion.Nombre)
                .ThenBy(o => o.GranCliente.Organizacion.Nombre);

            return await query.ProjectTo<ResponsableGranClienteListing>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<List<AgenteListing>> GetAgenteListingAsync()
        {
            var query = _data.Agentes.OrderBy(o => o.Codigo).Include(i => i.Organizacion).Include(i => i.TipoAgente);

            return await query.ProjectTo<AgenteListing>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obsoleto: Utilizar Task&lt;List&lt;TModel&gt;&gt; AgenteService.GetByTipoAsync&lt;TModel&gt;(string TipoAgenteId).
        /// </summary>
        public async Task<List<SelectListItem>> GetForSelectByTipoAsync(string TipoAgenteId)
        {
            var query = _data.Agentes.Where(w => w.TipoAgenteId == TipoAgenteId).OrderBy(o => o.Organizacion.Nombre);

            return await query.ProjectTo<SelectListItem>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<List<TModel>> GetByTipoAsync<TModel>(string TipoAgenteId) where TModel : class
        {
            var query = _data.Agentes.Where(w => w.TipoAgenteId == TipoAgenteId).OrderBy(o => o.Organizacion.Nombre).AsQueryable();

            return await query.ProjectTo<TModel>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }
    }
}

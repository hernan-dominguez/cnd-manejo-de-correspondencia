using AutoMapper;
using AutoMapper.QueryableExtensions;
using CWA.AccessControl;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using CWA.Entities.Identity;
using CWA.Models.Comun;
using CWA.Shared.Helpers;
using CWA.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CWA.Entities.Comun;
using Microsoft.Extensions.Logging;

namespace CWA.Application.Services.Comun
{
    public class CatalogoService : BaseService
    {
        public CatalogoService(DataContext Context, AppAccessControlService Access, IMapper Mapper, ILogger<BaseService> Logger) : base(Context, Access, Mapper, Logger) { }

        public async Task<CatalogoListing> GetCatalogoAsync(string Id)
        {
            var query = await _data.Catalogos.Where(w => w.Id == Id).FirstOrDefaultAsync();

            return _mapper.Map<CatalogoListing>(query);
        }

        public async Task<List<CatalogoListing>> GetCatalogoListingAsync()
        {
            var query = _data.Catalogos.OrderBy(o => o.Grupo).ThenBy(t => t.Id);

            return await query.ProjectTo<CatalogoListing>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<PaginatedList<CatalogoListing>> GetPaginatedAsync(int PageIndex, int PageSize, string SortingOrder, string FilteringText)
        {
            var query = _data.Catalogos.AsQueryable();

            // Filtering
            if (!FilteringText.Empty())
            {
                FilteringText = FilteringText.ToLower();

                query = query
                    .Where(w =>
                        w.Id.ToLower().Contains(FilteringText) ||
                        w.Grupo.ToLower().Contains(FilteringText) ||
                        w.Descripcion.ToLower().Contains(FilteringText) ||
                        (w.RefVal1 != null && w.RefVal1.ToLower().Contains(FilteringText)) ||
                        (w.RefVal2 != null && w.RefVal2.ToLower().Contains(FilteringText)) ||
                        (w.RefVal3 != null && w.RefVal3.ToLower().Contains(FilteringText)) ||
                        (w.RefVal4 != null && w.RefVal4.ToLower().Contains(FilteringText)) ||
                        (w.RefVal5 != null && w.RefVal5.ToLower().Contains(FilteringText)));
            }

            // Sorting
            query = SortingOrder switch
            {
                "id" => query.OrderBy(o => o.Id),
                "id_desc" => query.OrderByDescending(o => o.Id),
                "grupo" => query.OrderBy(o => o.Grupo),
                "grupo_desc" => query.OrderByDescending(o => o.Grupo),
                "descripcion" => query.OrderBy(o => o.Descripcion),
                "descripcion_desc" => query.OrderByDescending(o => o.Descripcion),
                _ => query.OrderBy(o => o.Grupo).ThenBy(t => t.Descripcion),
            };

            // Paginating
            return await base.GetPaginatedListAsync<CatalogoListing, Catalogo>(query, PageIndex, PageSize);
        }

        public async Task<List<CatalogoListing>> GetCatalogoListingAsync(string Grupo)
        {
            var query = _data.Catalogos.Where(w => w.Grupo == Grupo).OrderBy(o => o.Grupo).ThenBy(t => t.Id);

            return await query.ProjectTo<CatalogoListing>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<List<SelectListItem>> GetForSelectByGrupoAsync(string Grupo)
        {
            var query = _data.Catalogos.Where(w => w.Grupo.ToUpper() == Grupo.ToUpper().Trim()).OrderBy(o => o.Descripcion);

            return await query.ProjectTo<SelectListItem>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<List<CatalogoListing>> GetByFilterAsync(string Grupo = "", string RefVal1 = "", string RefVal2 = "", string RefVal3 = "", string RefVal4 = "", string RefVal5 = "", bool AllItems = false)
        {
            var query = _data.Catalogos.Where(w => (w.Grupo == Grupo || Grupo.Empty())
                && (w.RefVal1 == RefVal1 || RefVal1.Empty())
                && (w.RefVal2 == RefVal2 || RefVal2.Empty())
                && (w.RefVal3 == RefVal3 || RefVal3.Empty())
                && (w.RefVal4 == RefVal4 || RefVal4.Empty())
                && (w.RefVal5 == RefVal5 || RefVal5.Empty())
            );

            if (!AllItems) query = query.Where(w => w.Habilitado);

            return await query.ProjectTo<CatalogoListing>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<bool> AnyAsync(string Id = "", string Grupo = "", string RefVal1 = "", string RefVal2 = "", string RefVal3 = "", string RefVal4 = "", string RefVal5 = "")
        {
            var query = await _data.Catalogos.Where(w => (w.Id == Id || Id.Empty())
                && (w.Grupo == Grupo || Grupo.Empty())
                && (w.RefVal1 == RefVal1 || RefVal1.Empty())
                && (w.RefVal2 == RefVal2 || RefVal2.Empty())
                && (w.RefVal3 == RefVal3 || RefVal3.Empty())
                && (w.RefVal4 == RefVal4 || RefVal4.Empty())
                && (w.RefVal5 == RefVal5 || RefVal5.Empty())
            ).FirstOrDefaultAsync();

            return (query is not null);
        }

        public async Task<List<SelectListItem>> GetForSelectByGrupoRefAsync(string Grupo, string val)
        {
            var query = _data.Catalogos.Where(w => w.Grupo.ToUpper() == Grupo.ToUpper().Trim() && w.RefVal1.ToString() == val).OrderBy(o => o.Descripcion);

            return await query.ProjectTo<SelectListItem>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }
    }
}

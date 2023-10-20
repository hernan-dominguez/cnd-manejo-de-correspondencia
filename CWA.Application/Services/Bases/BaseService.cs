using AutoMapper;
using AutoMapper.QueryableExtensions;
using CWA.AccessControl;
using CWA.AccessControl.Services;
using CWA.Application.Services.Comun;
using CWA.Data;
using CWA.Entities.Identity;
using CWA.Models.Comun;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CWA.Application.Services.Bases
{
    public class BaseService
    {
        protected DataContext _data { get; }
        protected IMapper _mapper { get; }
        protected AppAccessControlService _access;
        protected ILogger<BaseService> _log;

        public BaseService(DataContext Data, AppAccessControlService Access, IMapper Mapper, ILogger<BaseService> Logger)
        {
            _data = Data;
            _access = Access;
            _mapper = Mapper;
            _log = Logger;
        }

        internal async Task<PaginatedList<TModel>> GetPaginatedListAsync<TModel, TEntity>(IQueryable<TEntity> SourceQuery, int PageIndex, int PageSize) where TModel : class where TEntity : class
        {
            // All items count after sorting and filtering applied
            var sourceCount = await SourceQuery.CountAsync();

            // Projected items for the specified page
            var query = SourceQuery.Skip((PageIndex - 1) * PageSize).Take(PageSize);

            // Querying database and mapping to final model
            var queryResult = await query.ProjectTo<TModel>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();

            // Convert to paginated collection
            return new PaginatedList<TModel>(queryResult, sourceCount, PageIndex, PageSize);
        }
    }
}

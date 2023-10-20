using AutoMapper;
using AutoMapper.QueryableExtensions;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CWA.Application.Services.Comun
{
    public class AgenteRegionalService : BaseService
    {
        public AgenteRegionalService(DataContext Context, AppAccessControlService Access, IMapper Mapper, ILogger<BaseService> Logger) : base(Context, Access, Mapper, Logger) { }

        public async Task<List<T>> GetAgenteRegionalListAsync<T>() where T : class
        {
            var query = _data.AgentesRegionales.OrderBy(o => o.Nombre);

            return await query.ProjectTo<T>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.Application.Services.Comun;
using CWA.Models.Comun;
using CWA.Shared.Extensions;
using CWA.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace CWA.Web.Pages.Admin
{
    public class CatalogosModel : PageModel
    {
        public CatalogoService _catalogos;
        public IOptions<ApplicationSettings> _options;

        public PaginatedList<CatalogoListing> CatalogoList { get; set; }

        public string IdSort { get; set; }

        public string GrupoSort { get; set; }

        public string DescripcionSort { get; set; }

        public string CurrentSort { get; set; }

        public string CurrentFilter { get; set; }

        public CatalogosModel(CatalogoService Catalogos, IOptions<ApplicationSettings> Options)
        {
            _catalogos = Catalogos;
            _options = Options;
        }

        public async Task OnGetAsync(string SortOrder, string SearchString, int? PageIndex)
        {
            IdSort = SortOrder == "id" ? "id_desc" : "id";
            GrupoSort = SortOrder == "grupo" ? "grupo_desc" : "grupo";
            DescripcionSort = SortOrder == "descripcion" ? "descripcion_desc" : "descripcion";
                        
            CurrentFilter = SearchString;
            CurrentSort = SortOrder;

            CatalogoList = await _catalogos.GetPaginatedAsync(PageIndex ?? 1, _options.Value.PageSize, CurrentSort, CurrentFilter);
        }
    }
}

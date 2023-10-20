using CWA.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Bases
{
    public interface IAtencion
    {
        public int? UsuarioAtencionId { get; set; }

        public DateTime? FechaAtencion { get; set; }

        public AppUser UsuarioAtencion { get; set; }
    }
}

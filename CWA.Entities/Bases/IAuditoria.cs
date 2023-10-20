using CWA.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Bases
{
    public interface IAuditoria
    {
        public DateTime RegFecha { get; set; }

        public int RegUsuarioId { get; set; }

        public DateTime ModFecha { get; set; }

        public int ModUsuarioId { get; set; }

        public AppUser RegUsuario { get; set; }

        public AppUser ModUsuario { get; set; }
    }
}

using CWA.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Bases.Protecciones
{
    public interface IPROTAprobacion
    {
        public int? UsuarioApruebaId { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        public AppUser UsuarioAprueba { get; set; }
    }
}

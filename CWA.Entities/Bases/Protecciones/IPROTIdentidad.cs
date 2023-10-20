using CWA.Entities.Protecciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Bases
{
    public interface IPROTIdentidad
    {
        public int RegistroId { get; set; }

        public PROTRegistro Registro { get; set; }
    }
}

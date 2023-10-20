using CWA.Entities.GrandesClientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Bases.GrandesClientes
{
    public interface IGCIdentidad
    {
        public int RegistroId { get; set; }

        public GCRegistro Registro { get; set; }
    }
}

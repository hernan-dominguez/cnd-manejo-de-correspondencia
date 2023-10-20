using CWA.Entities.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Bases.GrandesClientes
{
    public interface IGCMedicion
    {
        public int DistribuidoraId { get; set; }

        public string TipoDistIdentificacionId { get; set; }

        public string Cliente { get; set; }

        public string Identificacion { get; set; }

        public string TensionId { get; set; }

        public string Serie { get; set; }

        public Agente Distribuidora { get; set; }

        public Catalogo TipoDistIdentificacion { get; set; }

        public Catalogo Tension { get; set; }
    }
}

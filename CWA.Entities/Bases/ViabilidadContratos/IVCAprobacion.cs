using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Bases.ViabilidadContratos
{
    public interface IVCAprobacion
    {
        public bool? Aprobacion { get; set; }

        public string MotivoRechazo { get; set; }        
    }
}

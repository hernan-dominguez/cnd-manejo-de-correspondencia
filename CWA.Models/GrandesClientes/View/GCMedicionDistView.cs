using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCMedicionDistView
    {
        [Display(Name = "Serie")]
        public string Serie { get; set; }

        [Display(Name = "Nivel de Tensión")]
        public string Tension { get; set; }

        [Display(Name = "Distribuidora")]
        public string Distribuidora { get; set; }

        [Display(Name = "No. Cliente")]
        public string Cliente { get; set; }

        [Display(Name = "No. Identificación")]
        public string Identificacion { get; set; }

        public string TipoDistIdentificacion { get; set; }

        [Display(Name = "Circuito")]
        public string Circuito { get; set; }

        public DateTime ModFecha { get; set; }

        public DateTime? FechaAprobacion { get; set; }
    }
}

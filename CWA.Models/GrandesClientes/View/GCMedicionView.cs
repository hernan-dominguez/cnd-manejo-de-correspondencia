using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCMedicionView
    {
        [Display(Name = "Tipo de Conexión")]
        public string TipoConexion { get; set; }

        [Display(Name = "Serie")]
        public string Serie { get; set; }

        [Display(Name = "Nivel de Tensión")]
        public string Tension { get; set; }

        [Display(Name = "Distribuidora")]
        public string Distribuidora { get; set; }

        [Display(Name = "No. Identificación")]
        public string Identificacion { get; set; }

        public string TipoDistIdentificacion { get; set; }

        [Display(Name = "Subestación")]
        public string Subestacion { get; set; }

        [Display(Name = "Línea")]
        public string Linea { get; set; }

        [Display(Name = "Circuito")]
        public string Circuito { get; set; }

        public DateTime ModFecha { get; set; }

        public DateTime? FechaAprobacion { get; set; }
    }
}

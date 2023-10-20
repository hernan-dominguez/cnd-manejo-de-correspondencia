using CWA.Models.GrandesClientes.Edit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCMedidorDistView
    {
        [Display(Name = "Fabricante")]
        public string Fabricante { get; set; }

        [Display(Name = "Modelo")]
        public string Modelo { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Relación CT")]
        public string RelacionCT { get; set; }

        [Display(Name = "Relación PT")]
        public string RelacionPT { get; set; }

        [Display(Name = "Contraseña")]
        public string Clave { get; set; }

        [Display(Name = "Comunicación (IP o Teléfono)")]
        public string Acceso { get; set; }

        [Display(Name = "MID/TID")]
        public string MidTid { get; set; }

        [Display(Name = "Canales")]
        public List<GCMedidorCanalView> Canales { get; set; }

        public DateTime ModFecha { get; set; }

        public DateTime? FechaAprobacion { get; set; }
    }
}
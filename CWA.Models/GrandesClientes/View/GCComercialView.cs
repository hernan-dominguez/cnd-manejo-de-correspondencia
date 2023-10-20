using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCComercialView
    {
        [Display(Name = "Cuenta Bancaria")]
        public string CuentaBancaria { get; set; }

        [Display(Name = "Monto de Garantía Nacional")]
        public double? MontoGarantiaNacional { get; set; }

        [Display(Name = "Monto de Garantía Regional")]
        public double? MontoGarantiaRegional { get; set; }
        
        public DateTime ModFecha { get; set; }

        public DateTime? FechaAprobacion { get; set; }
    }
}

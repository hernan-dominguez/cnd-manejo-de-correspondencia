using CWA.Shared.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.Edit
{
    public class GCComercialEdit
    {
        [RegularExpression("^[0-9]*$")]
        [StringLength(100)]
        [Display(Name = "Cuenta Bancaria")]
        public string CuentaBancaria { get; set; }

        [Range(1, double.MaxValue)]
        [Display(Name = "Monto de Garantía Nacional")]
        public double? MontoGarantiaNacional { get; set; }

        [Range(1, double.MaxValue)]
        [Display(Name = "Monto de Garantía Regional")]
        public double? MontoGarantiaRegional { get; set; }

        public DateTime ModFecha { get; set; }
    }
}

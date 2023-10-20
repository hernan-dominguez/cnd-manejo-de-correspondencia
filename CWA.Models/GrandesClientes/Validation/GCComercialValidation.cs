using CWA.Shared.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.Validation
{
    public class GCComercialValidation
    {
        [Required]
        [AlphaNumberDash]
        [StringLength(100)]
        public string CuentaBancaria { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public double? MontoGarantiaNacional { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public double? MontoGarantiaRegional { get; set; }

        public DateTime ModFecha { get; set; }
    }
}

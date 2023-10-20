using CWA.Shared.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.Edit
{
    public class GCMedicionDistEdit
    {
        [Required]
        [AlphaNumberDash]
        [StringLength(100)]
        [Display(Name = "Serie")]
        public string Serie { get; set; }

        [Required]
        [Display(Name = "Nivel de Tensión")]
        public string TensionId { get; set; }

        [Required]
        [Display(Name = "Distribuidora")]
        public string DistribuidoraId { get; set; }

        [Required]
        public string TipoDistIdentificacionId { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression("^[0-9]*$")]
        [Display(Name = "No. Cliente")]
        public string Cliente { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression("^[0-9]*$")]
        [Display(Name = "No. Identificación")]
        public string Identificacion { get; set; }
        
        [Required]
        [StringLength(500)]
        [Display(Name = "Circuito")]
        public string Circuito { get; set; }

        public DateTime ModFecha { get; set; }    
    }
}

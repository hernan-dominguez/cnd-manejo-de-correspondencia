using CWA.Shared.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace CWA.Models.GrandesClientes.Edit
{
    public class GCMedicionSmecEdit
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
        [Display(Name = "Subestación")]
        public string SubestacionId { get; set; }

        [Required]
        [Display(Name = "Línea")]
        public string LineaId { get; set; }

        public DateTime ModFecha { get; set; }
    }
}
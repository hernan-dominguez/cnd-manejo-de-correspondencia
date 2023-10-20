using CWA.Shared.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace CWA.Models.GrandesClientes.Edit
{
    public class GCMedicionEdit
    {
        [Required]
        [Display(Name = "Tipo de Conexión")]
        public string TipoConexionId { get; set; }

        [Required]
        [AlphaNumberDash]
        [StringLength(100)]
        [Display(Name = "Serie")]
        public string Serie { get; set; }

        [Required]
        [Display(Name = "Nivel de Tensión")]
        public string TensionId { get; set; }
                
        [Display(Name = "Distribuidora para contratar potencia")]
        public string DistribuidoraId { get; set; }

        public string TipoDistIdentificacionId { get; set; }
                
        [StringLength(100)]
        [RegularExpression("^[0-9]*$")]
        [Display(Name = "No. Identificación")]
        public string Identificacion { get; set; }
                
        [Display(Name = "Subestación")]
        public string SubestacionId { get; set; }

        [Display(Name = "Línea")]
        public string LineaId { get; set; }

        [StringLength(500)]
        [Display(Name = "Circuito")]
        public string Circuito { get; set; }

        public DateTime ModFecha { get; set; }
    }
}

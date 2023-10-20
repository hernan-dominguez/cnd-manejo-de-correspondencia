using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.ViabilidadContratos.Edit
{
    public class VCNacionalEdit
    {
        [Required]
        [Display(Name = "Tipo de Contrato")]
        public string TipoContratoId { get; set; }
                
        [Display(Name = "Vendedor")]
        public int VendedorId { get; set; }
                
        [Display(Name = "Comprador")]
        public int CompradorId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Inicia")]
        public DateTime Inicia { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Finaliza")]
        public DateTime Finaliza { get; set; }

        [Required]
        [Display(Name = "Documentos requeridos")]
        public List<VCDocumentoEdit> Documentos { get; set; }

        public string DocsPath { get; set; }

        public string TempPath { get; set; }

        public string NotificacionId { get; set; }
    }
}

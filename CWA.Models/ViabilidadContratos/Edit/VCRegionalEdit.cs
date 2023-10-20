using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.ViabilidadContratos.Edit
{
    public class VCRegionalEdit
    {
        [Required]
        [Display(Name = "Tipo de Solicitud")]
        public string TipoSolicitudId { get; set; }

        [Required]
        [Display(Name = "Tipo de Transacción")]
        public string TipoTransaccionId { get; set; }
                
        [Display(Name ="Agente Solicitante")]
        public int SolicitanteId { get; set; }

        [Required]
        [Display(Name = "País")]
        public string PaisId { get; set; }
                
        [Display(Name ="Agente Regional")]
        public int ContraparteId { get; set; }

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

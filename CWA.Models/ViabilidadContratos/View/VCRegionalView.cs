using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.ViabilidadContratos.View
{
    public class VCRegionalView
    {
        public int Id { get; set; }

        public string TipoSolicitudId { get; set; }

        public bool? Aprobacion { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Display(Name = "Solicitud")]
        public string Solicitud { get; set; }

        [Display(Name = "Transacción")]
        public string Transaccion { get; set; }

        [Display(Name = "Solicitante")]
        public string Solicitante { get; set; }

        [Display(Name = "País")]
        public string Pais { get; set; }

        [Display(Name = "Contraparte")]
        public string Contraparte { get; set; }

        [Display(Name = "Contraparte")]
        public string ContraparteNombre { get; set; }

        [Display(Name = "Inicia")]
        public DateTime Inicia { get; set; }

        [Display(Name = "Finaliza")]
        public DateTime Finaliza { get; set; }

        [Display(Name = "Atendido")]
        public DateTime? FechaAtencion { get; set; }

        [Display(Name = "Registrado")]
        public DateTime RegFecha { get; set; }

        [Display(Name = "Documentos")]
        public List<VCDocumentoView> Documentos;

        [Display(Name = "Motivo del rechazo")]
        public string MotivoRechazo { get; set; }
    }
}

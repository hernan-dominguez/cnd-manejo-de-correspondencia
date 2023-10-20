using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.ViabilidadContratos.View
{
    public class VCNacionalView
    {
        public int Id { get; set; }

        public string TipoContratoId { get; set; }

        public bool? Aprobacion { get; set; }
                
        public string Tipo { get; set; }

        public string CompradorNombre { get; set; }

        public string VendedorNombre { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Display(Name = "Vendedor")]
        public string Vendedor { get; set; }

        [Display(Name = "Comprador")]
        public string Comprador { get; set; }

        [Display(Name = "Inicia")]
        public DateTime Inicia { get; set; }

        [Display(Name = "Finaliza")]
        public DateTime Finaliza { get; set; }

        [Display(Name = "Atendido")]
        public DateTime? FechaAtencion { get; set; }

        [Display(Name = "Registrado")]
        public DateTime RegFecha { get; set; }

        [Display(Name = "Documentos")]
        public List<VCDocumentoView> Documentos { get; set; }

        [Display(Name = "Motivo del rechazo")]
        public string MotivoRechazo { get; set; }
    }
}

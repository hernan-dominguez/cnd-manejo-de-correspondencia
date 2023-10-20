using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.ViabilidadContratos.View
{
    public class VCEnmiendaView : VCNacionalView
    {
        public int ContratoId { get; set; }

        [Display(Name ="Código de Contrato")]
        public string CodigoContrato { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCDocumentosView
    {
        public int Id { get; set; }

        public string TipoDocumentoId { get; set; }

        [Display(Name = "Documento")]
        public string Descripcion { get; set; }

        [Display(Name = "Actualizado")]
        public DateTime ModFecha { get; set; }

        [Display(Name = "Fecha de aprobación")]
        public DateTime? FechaAtencion { get; set; }
    }
}

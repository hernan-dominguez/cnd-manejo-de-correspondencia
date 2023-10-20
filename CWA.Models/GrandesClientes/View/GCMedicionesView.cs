using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCMedicionesView
    {
        public int Id { get; set; }

        [Display(Name = "Tipo de Conexión")]
        public string Tipo { get; set; }

        [Display(Name = "Serie (Medidor)")]
        public string Serie { get; set; }

        [Display(Name = "Fecha de aprobación")]
        public DateTime? FechaAtencion { get; set; }

        [Display(Name = "Actualizado")]
        public DateTime ModFecha { get; set; }
    }
}

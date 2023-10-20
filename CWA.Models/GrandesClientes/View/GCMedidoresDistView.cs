using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCMedidoresDistView
    {
        public int Id { get; set; }

        [Display(Name = "Distribuidora")]
        public string Origen { get; private set; }

        [Display(Name = "Serie")]
        public string Serie { get; set; }

        [Display(Name = "Fecha de aprobación")]
        public DateTime? FechaAprobacion { get; set; }

        [Display(Name = "Actualizado")]
        public DateTime ModFecha { get; set; }

        public GCMedidoresDistView(string OrigenMedicion) => Origen = OrigenMedicion;
    }
}

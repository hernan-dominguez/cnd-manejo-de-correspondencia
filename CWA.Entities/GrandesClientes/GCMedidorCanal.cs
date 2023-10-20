using CWA.Entities.Comun;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.GrandesClientes
{
    public class GCMedidorCanal
    {
        public int MedidorId { get; set; }

        public int Numero { get; set; }

        [Required]
        [StringLength(7)]
        public string DescripcionId { get; set; }

        public GCMedidorDist Medidor { get; set; }

        public Catalogo Descripcion { get; set; }
    }
}
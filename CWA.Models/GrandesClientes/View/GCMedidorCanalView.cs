using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCMedidorCanalView
    {
        [Display(Name = "Número")]
        public int Numero { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.Protecciones.View
{
    public class PROTPlantillasView
    {
        public int Id { get; set; }

        [Display(Name = "Documento")]
        public string Descripcion { get; set; }

        [Display(Name = "Actualizado")]
        public DateTime ModFecha { get; set; }
    }
}

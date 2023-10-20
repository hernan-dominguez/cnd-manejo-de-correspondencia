using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CWA.Models.ManejoDeCorrespondencia.Carga
{
    public class MCSubtema
    {
        [Display(Name = "Tema")]
        public string Tema { get; set; }

        [Display(Name = "Id Subtema")]
        public string IdSubtema { get; set; }

        [Display(Name = "Nombre Subtema")]
        public string NombreSubtema { get; set; }

    }
}

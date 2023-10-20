using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CWA.Models.ManejoDeCorrespondencia.Carga
{
    public class MCAgente
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Display(Name = "Tipo de Agente")]
        public string TipoAgente { get; set; }

        [Display(Name = "Nombre de Agente")]
        public string NombreAgente { get; set; }

    }
}

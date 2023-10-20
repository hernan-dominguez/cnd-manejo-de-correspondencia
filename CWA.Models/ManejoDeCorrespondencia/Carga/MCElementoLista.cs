using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CWA.Models.ManejoDeCorrespondencia.Carga
{
    public class MCElementoLista
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "Valor")]
        public string Valor { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CWA.Models.ManejoDeCorrespondencia.Carga
{
    public class MCNotaSaliente
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "Número de Nota")]
        public string NumeroDeNota { get; set; }

        [Display(Name = "Destinatarios")]
        public List<string> Destinatarios { get; set; }
    }
}

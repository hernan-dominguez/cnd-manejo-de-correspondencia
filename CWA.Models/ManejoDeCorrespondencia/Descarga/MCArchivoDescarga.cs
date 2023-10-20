using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CWA.Models.ManejoDeCorrespondencia.Descarga
{
    public class MCArchivoDescarga
    {
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Número de Nota")]
        public string NumeroDeNota { get; set; }

        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }

        [Display(Name = "Fecha")]
        public string Fecha { get; set; }
    }
}

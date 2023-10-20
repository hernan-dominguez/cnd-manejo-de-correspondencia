using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.Comun
{
    public class CatalogoListing
    {
        [Display(Name = "Grupo")]
        public string Grupo { get; set; }

        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Valor 1")]
        public string RefVal1 { get; set; }

        [Display(Name = "Valor 2")]
        public string RefVal2 { get; set; }

        [Display(Name = "Valor 3")]
        public string RefVal3 { get; set; }

        [Display(Name = "Valor 4")]
        public string RefVal4 { get; set; }

        [Display(Name = "Valor 5")]
        public string RefVal5 { get; set; }
    }
}

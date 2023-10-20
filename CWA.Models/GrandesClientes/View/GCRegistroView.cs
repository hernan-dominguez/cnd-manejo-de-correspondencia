using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCRegistroView
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Tipo")]
        public string Tipo { get; set; }

        [Display(Name = "Registrante")]
        public string Registrante { get; set; }

        [Display(Name = "Fecha de Entrada")]
        public DateTime? FechaEntrada { get; set; }
                
        [Display(Name = "Estatus")]
        public string Estatus { get; set; }
                
        [Display(Name = "Nombre")]
        public string ContactoNombre { get; set; }
        
        [Display(Name = "Teléfono")]
        public string ContactoTelefono { get; set; }
        
        [Display(Name = "Correo electrónico")]
        public string ContactoCorreo { get; set; }

        public bool FechaEditable { get; set; }

        [Display(Name = "Fecha de Autorización")]
        public DateTime? FechaAtencion{ get; set; }

        public DateTime ModFecha { get; set; }
    }
}

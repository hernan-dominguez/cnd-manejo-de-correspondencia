using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCGeneralesView
    {
        [Display(Name = "Razón Social")]
        public string RazonSocial { get; set; }

        [Display(Name = "RUC")]
        public string Ruc { get; set; }

        [Display(Name = "Dígito Verificador")]
        public int? Digito { get; set; }

        [Display(Name = "Provincia")]
        public string Provincia { get; set; }

        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Display(Name = "Sitio Web")]
        public string WebUrl { get; set; }

        [Display(Name = "Nombre")]
        public string LegalNombre { get; set; }

        [Display(Name = "Correo")]
        public string LegalCorreo { get; set; }

        [Display(Name = "Teléfono")]
        public string LegalTelefono { get; set; }

        [Display(Name = "Nombre")]
        public string SmecNombre { get; set; }

        [Display(Name = "Correo")]
        public string SmecCorreo { get; set; }

        [Display(Name = "Teléfono")]
        public string SmecTelefono { get; set; }

        public DateTime ModFecha { get; set; }

        public DateTime? FechaAprobacion { get; set; }
    }
}

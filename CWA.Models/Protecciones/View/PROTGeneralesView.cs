using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.Protecciones.View
{
    public class PROTGeneralesView
    {
        [Display(Name = "Dígito Verificador")]
        public int? DigitoVerificador { get; set; }

        [Display(Name = "Registro Único de Contribuyente")]
        public string RegistroUnicoContribuyente { get; set; }

        [Display(Name = "Dirección Fisica")]
        public string DireccionFisica { get; set; }

        [Display(Name = "Provincia")]
        public string Provincia { get; set; }

        [Display(Name = "Nombre")]
        public string RLegalNombre { get; set; }

        [Display(Name = "Teléfono")]
        public string RLegalTel { get; set; }

        [Display(Name = "Correo")]
        public string RLegalCorreo { get; set; }

        [Display(Name = "Nombre")]
        public string RProteccionNombre { get; set; }

        [Display(Name = "Teléfono")]
        public string RProteccionTel { get; set; }

        [Display(Name = "Correo")]
        public string RProteccionCorreo { get; set; }

        public DateTime ModFecha { get; set; }

        public DateTime? FechaAprobacion { get; set; }
    }
}

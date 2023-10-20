//using CWA.Entities.Protecciones;
using CWA.Shared.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.Protecciones.Edit
{
    public class PROTGeneralesEdit
    {
        //public PROTGenerales PROTGenerales { get; set; }

        [Required]
        [Display(Name = "Dígito Verificador")]
        public int? DigitoVerificador { get; set; }

        [Required]
        [Display(Name = "Dirección Fisica")]
        public string DireccionFisica { get; set; }

        [Required]
        [Display(Name = "Provincia")]
        public string ProvinciaId { get; set; }

        [Required]
        [Display(Name = "Registro Unico del Contribuyente")]
        public string RegistroUnicoContribuyente { get; set; }

        [Required]
        [PersonName]
        [Display(Name = "Nombre")]
        public string RLegalNombre { get; set; }

        [Required]
        [PhoneOrIPv4]
        [Display(Name = "Teléfono")]
        public string RLegalTel { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string RLegalCorreo { get; set; }

        [Required]
        [PersonName]
        [Display(Name = "Nombre")]
        public string RProteccionNombre { get; set; }

        [Required]
        [PhoneOrIPv4]
        [Display(Name = "Teléfono")]
        public string RProteccionTel { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string RProteccionCorreo { get; set; }

        public DateTime ModFecha { get; set; }
    }
}

using CWA.Shared.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.Edit
{
    public class GCGeneralesEdit
    {
        [Required]
        [StringLength(500)]
        [Display(Name = "Razón Social")]
        public string RazonSocial { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "RUC")]
        public string Ruc { get; set; }

        [Required]
        [Display(Name = "Dígito Verificador")]
        public int? Digito { get; set; }

        [Required]
        [Display(Name = "Provincia")]
        public string ProvinciaId { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        [PhoneOrIPv4]
        [StringLength(100)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }
        
        [Url]
        [StringLength(100)]
        [Display(Name = "Sitio Web")]
        public string WebUrl { get; set; }

        [Required]
        [PersonName]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string LegalNombre { get; set; }

        [Required]
        [PhoneOrIPv4]
        [StringLength(100)]
        [Display(Name = "Teléfono")]
        public string LegalTelefono { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Correo")]
        public string LegalCorreo { get; set; }

        [Required]
        [PersonName]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string SmecNombre { get; set; }

        [Required]
        [PhoneOrIPv4]
        [StringLength(100)]
        [Display(Name = "Teléfono")]
        public string SmecTelefono { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Correo")]
        public string SmecCorreo { get; set; }

        public DateTime ModFecha { get; set; }
    }
}

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
    public class PROTRegistroEdit
    {
        [Required]
        [StringLength(500)]
        [Display(Name = "Nombre del Agente")]
        public string RazonSocial { get; set; }

        [Required]
        [Display(Name = "Tipo de Gran Cliente")]
        public string TipoAgenteId { get; set; }

        [Required]
        [PersonName]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string ContactoNombre { get; set; }

        [Required]
        [PhoneOrIPv4]
        [StringLength(100)]
        [Display(Name = "Teléfono")]
        public string ContactoTelefono { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Correo electrónico")]
        public string ContactoCorreo { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string UsuarioID { get; set; }
    }
}

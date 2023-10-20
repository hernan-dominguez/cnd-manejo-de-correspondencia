using CWA.Shared.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.Edit
{
    public class GCRegistroEdit
    {
        [Required]
        [StringLength(500)]
        [Display(Name = "Nombre del Gran Cliente")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Tipo de Gran Cliente")]
        public string TipoGranClienteId { get; set; }

        [Display(Name = "Agente Responsable")]
        public string ResponsableId { get; set; }

        [Display(Name = "Usuario Propietario")]
        public string PropietarioId { get; set; }

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
    }
}

using CWA.Shared.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.Edit
{
    public class GCMedidorDistEdit
    {
        [Required]
        [Display(Name = "Fabricante")]
        public string FabricanteId { get; set; }

        [Required]
        [Display(Name = "Modelo")]
        public string ModeloId { get; set; }

        [StringLength(100)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Relación CT")]
        public string RelacionCT { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Relación PT")]
        public string RelacionPT { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Contraseña")]
        public string Clave { get; set; }

        [Required]
        [PhoneOrIPv4(AccessType = PhoneOrIPv4Formats.PhoneOrIPv4)]
        [StringLength(100)]
        [Display(Name = "Comunicación (IP o Teléfono)")]
        public string Acceso { get; set; }

        [StringLength(100)]
        [Display(Name = "MID/TID")]
        public string MidTid { get; set; }

        [Required]
        [Display(Name = "Canales")]
        public List<GCMedidorCanalEdit> Canales { get; set; }

        public DateTime LastSave { get; set; }
    }
}

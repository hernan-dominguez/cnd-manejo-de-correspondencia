using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.Protecciones.View
{
    public class PROTRegistroView
    {
        public int Id { get; set; }
        public string RegUsuarioID { get; set; }
        [Display(Name = "Registrador Por")]
        public string RegUsuario { get; set; }
        [Display(Name = "Razon Social")]
        public string RazonSocial { get; set; }
        [Display(Name = "Tipo")]
        public string Tipo { get; set; }
        [Display(Name = "Dirección Física")]
        public string DireccionFisica { get; set; }
        public string Provincia { get; set; }
        [Display(Name = "Registro Único de Contribuyente")]
        public string RegistroUnicoContribuyente { get; set; }
        [Display(Name = "Dígito Verificador")]
        public string DigitoVerificador { get; set; }
        [Display(Name = "Representante Legal")]
        public string RLegalNombre { get; set; }
        [Display(Name = "Correo del Representante Legal")]
        public string RLegalCorreo { get; set; }
        [Display(Name = "Teléfono del Representante Legal")]
        public string RLegalTel { get; set; }
        [Display(Name = "Responsable de Protecciones")]
        public string RProteccionNombre { get; set; }
        [Display(Name = "Correo del Representante de Protecciones")]
        public string RProteccionCorreo { get; set; }
        [Display(Name = "Teléfono del Representante de Protecciones")]
        public string RProteccionTel { get; set; }

        [Display(Name = "Condición de Fecha")]
        public string FechaEstatus { get; set; }
        [Display(Name = "Fecha de Aprobación")]
        public DateTime? FechaAprobacion { get; set; }

        public DateTime RegFecha { get; set; }
    }
}

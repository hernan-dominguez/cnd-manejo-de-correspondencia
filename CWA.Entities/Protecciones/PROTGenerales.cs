using CWA.Entities.Bases;
using CWA.Entities.Bases.Protecciones;
using CWA.Entities.Comun;
using CWA.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Protecciones
{
    public class PROTGenerales : IIdentidad<int>, IPROTAprobacion, IAuditoria
    {
        //IIdentidad<int>
        public int Id { get; set; }
        [Display(Name = "Dirección Física")]
        public string DireccionFisica { get; set; }
        public Catalogo Provincia { get; set; }
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
        public string ProvinciaId { get; set; }

        //IAuditoria
        [Required]
        [Column(TypeName = "DATE")]
        public DateTime RegFecha { get; set; }
        public int RegUsuarioId { get; set; }

        //IPROTAprobacion
        public int? UsuarioApruebaId { get; set; }
        [Column(TypeName = "DATE")]
        public DateTime? FechaAprobacion { get; set; }
        public AppUser UsuarioAprueba { get; set; }
        [Required]
        [Column(TypeName = "DATE")]

        //Keys
        public DateTime ModFecha { get; set; }
        public int ModUsuarioId { get; set; }
        [ForeignKey("RegUsuarioId")]
        public AppUser RegUsuario { get; set; }
        [ForeignKey("ModUsuarioId")]
        public AppUser ModUsuario { get; set; }
    }
}

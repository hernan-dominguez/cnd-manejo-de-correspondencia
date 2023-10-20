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
    public class PROTRegistro : IIdentidad<int>, IAuditoria, IPROTAprobacion
    {
        public int Id { get; set; }

        [Required]
        public string RazonSocial { get; set; }

        [Required]
        public string TipoAgenteId { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactoNombre { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactoTelefono { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactoCorreo { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public int RegUsuarioId { get; set; }

        public int? GeneralesId { get; set; }

        public int? UsuarioApruebaId { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? FechaAprobacion { get; set; }

        [Required]
        [StringLength(7)]
        public string EstatusId { get; set; }

        public Catalogo Tipo { get; set; }

        public PROTGenerales Generales { get; set; }

        public AppUser UsuarioAprueba { get; set; }

        public ICollection<PROTDocumento> Documentos { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime RegFecha { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime ModFecha { get; set; }

        public int ModUsuarioId { get; set; }

        [ForeignKey("RegUsuarioId")]
        public AppUser RegUsuario { get; set; }

        [ForeignKey("ModUsuarioId")]
        public AppUser ModUsuario { get; set; }
    }
}

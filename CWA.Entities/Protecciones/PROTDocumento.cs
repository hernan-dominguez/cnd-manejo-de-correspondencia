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
    public class PROTDocumento : IIdentidad<int>, IPROTIdentidad, IPROTAprobacion, IAuditoria
    {
        public int Id { get; set; }

        public int RegistroId { get; set; }

        public bool Loaded { get; set; }

        [Required]
        public string Archivo { get; set; }

        [Required]
        public string Mostrar { get; set; }

        [Required]
        public string TipoDocumentoId { get; set; }

        public PROTRegistro Registro { get; set; }

        public PROTBitacoraDocumentos Bitacora { get; set; }

        public Catalogo TipoDocumento { get; set; }

        public int? UsuarioApruebaId { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? FechaAprobacion { get; set; }

        [ForeignKey("UsuarioApruebaId")]
        public AppUser UsuarioAprueba { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime RegFecha { get; set; }

        public int RegUsuarioId { get; set; }

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

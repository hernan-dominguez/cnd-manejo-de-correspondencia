using CWA.Entities.Bases;
using CWA.Entities.Bases.GrandesClientes;
using CWA.Entities.Comun;
using CWA.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.GrandesClientes
{
    public class GCDocumento : IIdentidad<int>, IGCIdentidad, IAtencion, IAuditoria
    {
        public int Id { get; set; }

        public int RegistroId { get; set; }

        public bool Loaded { get; set; }

        [Required]
        [StringLength(500)]
        public string Archivo { get; set; }

        [Required]
        [StringLength(500)]
        public string Mostrar { get; set; }

        [Required]
        [StringLength(7)]
        public string TipoDocumentoId { get; set; }

        public GCRegistro Registro { get; set; }

        public Catalogo TipoDocumento { get; set; }

        public int? UsuarioAtencionId { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? FechaAtencion { get; set; }
                
        public AppUser UsuarioAtencion { get; set; }

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

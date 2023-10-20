using CWA.Entities.Bases;
using CWA.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Comun
{
    public class CorreoMensaje : IIdentidad<int>, IAuditoria
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(4)]
        public string AppId { get; set; }

        public int Estatus { get; set; }

        [StringLength(25)]
        public string ItemKey { get; set; }

        [Required]
        [StringLength(100)]
        public string Destinatario { get; set; }

        [Required]
        [StringLength(500)]
        public string Asunto { get; set; }

        [Required]
        [StringLength(500)]
        public string Previa { get; set; }

        [Required]
        [StringLength(500)]
        public string Saludo { get; set; }

        [Required]
        [MaxLength(65536)]
        public string Contenido { get; set; }

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

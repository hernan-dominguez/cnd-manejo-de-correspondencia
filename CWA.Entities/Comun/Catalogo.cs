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
    public class Catalogo : IIdentidad<string>, IAuditoria
    {
        [StringLength(7)]
        public string Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Grupo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public string RefVal1 { get; set; }

        public string RefVal2 { get; set; }

        public string RefVal3 { get; set; }

        public string RefVal4 { get; set; }

        public string RefVal5 { get; set; }

        public bool Habilitado { get; set; }

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

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
    public class DiaIrregular : IIdentidad<int>, IAuditoria
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime Dia { get; set; }

        [Required]
        [StringLength(7)]
        public string CondicionId { get; set; }

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }

        public Catalogo Condicion { get; set; }
        
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

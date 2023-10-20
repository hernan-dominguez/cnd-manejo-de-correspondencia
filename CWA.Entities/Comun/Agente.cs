
using CWA.Entities.Bases;
using CWA.Entities.Comun;
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
    public class Agente : IIdentidad<int>, IAuditoria
    {
        public int Id { get; set; }

        [Required]
        [StringLength(6)]
        public string IdBdi { get; set; }

        [Required]
        public int OrganizacionId { get; set; }

        [Required]
        [StringLength(25)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(7)]
        public string TipoAgenteId { get; set; }                        

        public Organizacion Organizacion { get; set; }
        
        public Catalogo TipoAgente { get; set; }

        // Auditing
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

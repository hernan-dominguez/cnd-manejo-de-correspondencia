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
    public class Organizacion : IIdentidad<int>, IAuditoria
    {        
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }
        
        [StringLength(100)]
        public string ContactoNombre { get; set; }

        [StringLength(100)]
        public string ContactoTelefono { get; set; }

        [StringLength(100)]
        public string ContactoCorreo { get; set; }

        public Agente Agente { get; set; }
        
        public ICollection<UsuarioOrganizacion> Usuarios { get; set; }

        // Auditoría
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

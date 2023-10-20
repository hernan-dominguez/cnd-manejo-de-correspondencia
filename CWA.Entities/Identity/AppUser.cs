using CWA.Entities.Bases;
using CWA.Entities.Comun;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Identity
{
    public class AppUser : IdentityUser<int>
    {
        [Required]
        public string Nombre { get; set; }

        public bool Locked { get; set; }

        public UsuarioOrganizacion Organizacion { get; set; }

        public ICollection<AppUserClaim> Claims { get; set; }

        //Auditoría
        [Required]
        public int RegUsuarioId { get; set; }

        [Required]
        public int ModUsuarioId { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime RegFecha { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime ModFecha { get; set; }        
    }
}

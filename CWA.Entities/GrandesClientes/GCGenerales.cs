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
    public class GCGenerales : IIdentidad<int>, IAtencion, IAuditoria
    {
        public int Id { get; set; }

        [StringLength(500)]
        public string RazonSocial { get; set; }

        [StringLength(100)]
        public string Ruc { get; set; }

        public int? Digito { get; set; }

        [StringLength(7)]
        public string ProvinciaId { get; set; }

        [StringLength(500)]
        public string Direccion { get; set; }

        [StringLength(100)]
        public string Telefono { get; set; }

        [StringLength(100)]
        public string WebUrl { get; set; }

        [StringLength(100)]
        public string LegalNombre { get; set; }

        [StringLength(100)]
        public string LegalTelefono { get; set; }

        [StringLength(100)]
        public string LegalCorreo { get; set; }

        [StringLength(100)]
        public string SmecNombre { get; set; }

        [StringLength(100)]
        public string SmecTelefono { get; set; }

        [StringLength(100)]
        public string SmecCorreo { get; set; }

        public Catalogo Provincia { get; set; }

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

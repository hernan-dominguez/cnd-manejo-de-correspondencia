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

namespace CWA.Entities.GrandesClientes
{
    public class GCMedidorDist : IIdentidad<int>, IAtencion, IAuditoria
    {
        public int Id { get; set; }
                
        public string FabricanteId { get; set; }
                
        public string ModeloId { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(100)]
        public string RelacionCT { get; set; }

        [StringLength(100)]
        public string RelacionPT { get; set; }

        [StringLength(100)]
        public string MidTid { get; set; }

        [StringLength(100)]
        public string Acceso { get; set; }

        [StringLength(100)]
        public string Clave { get; set; }

        public Catalogo Fabricante { get; set; }

        public Catalogo Modelo { get; set; }

        public ICollection<GCMedidorCanal> Canales { get; set; }

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

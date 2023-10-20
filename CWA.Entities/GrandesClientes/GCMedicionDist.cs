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
    public class GCMedicionDist : IIdentidad<int>, IGCMedicion, IGCIdentidad, IAtencion, IAuditoria
    {
        public int Id { get; set; }

        public int RegistroId { get; set; }

        [Required]
        public int DistribuidoraId { get; set; }

        [Required]
        [StringLength(7)]
        public string TipoDistIdentificacionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Cliente { get; set; }

        [Required]
        [StringLength(100)]
        public string Identificacion { get; set; }

        [Required]
        [StringLength(500)]
        public string Circuito { get; set; }

        public int? MedidorId { get; set; }

        public GCRegistro Registro { get; set; }

        public Agente Distribuidora { get; set; }

        public Catalogo TipoDistIdentificacion { get; set; }

        public GCMedidorDist Medidor { get; set; }

        [Required]
        [StringLength(7)]
        public string TensionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Serie { get; set; }

        public Catalogo Tension { get; set; }

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

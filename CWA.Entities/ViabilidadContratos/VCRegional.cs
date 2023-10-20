using CWA.Entities.Bases;
using CWA.Entities.Bases.ViabilidadContratos;
using CWA.Entities.Comun;
using CWA.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.ViabilidadContratos
{
    public class VCRegional : IIdentidad<int>, IVCAprobacion, IAtencion, IAuditoria
    {
        public int Id { get; set; }

        [Required]
        [StringLength(7)]
        public string TipoSolicitudId { get; set; }

        [Required]
        [StringLength(7)]
        public string TipoTransaccionId { get; set; }

        public int SolicitanteId { get; set; }

        public int ContraparteId { get; set; }

        [Required]
        [StringLength(7)]
        public string PaisId { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime Inicia { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime Finaliza { get; set; }

        public bool? Aprobacion { get; set; }

        public int? UsuarioAtencionId { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? FechaAtencion { get; set; }

        [MaxLength(65536)]
        public string MotivoRechazo { get; set; }

        public Catalogo TipoSolicitud { get; set; }

        public Catalogo TipoTransaccion { get; set; }    

        public Agente Solicitante { get; set; }

        public Catalogo Pais { get; set; }

        public AgenteRegional Contraparte { get; set; }

        public AppUser UsuarioAtencion { get; set; }

        public ICollection<VCDocRegional> Documentos { get; set; }

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

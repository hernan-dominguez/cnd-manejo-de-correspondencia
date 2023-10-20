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
    public class VCNacional : IIdentidad<int>, IVCAprobacion, IAtencion, IAuditoria
    {
        public int Id { get; set; }

        // [Required]
        // public string Codigo { get; set; }
        // public string CodigoDisplay { get; set; }

        [Required]
        [StringLength(7)]
        public string TipoContratoId { get; set; }
    
        public int VendedorId { get; set; }
                        
        public int CompradorId { get; set; }

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

        public Catalogo TipoContrato { get; set; }

        public Agente Vendedor { get; set; }

        public Agente Comprador { get; set; }

        public AppUser UsuarioAtencion { get; set; }

        public ICollection<VCDocNacional> Documentos { get; set; }

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

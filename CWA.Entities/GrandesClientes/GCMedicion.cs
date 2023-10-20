using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CWA.Entities.Bases;
using CWA.Entities.Bases.GrandesClientes;
using CWA.Entities.Comun;
using CWA.Entities.Identity;

namespace CWA.Entities.GrandesClientes
{
    public class GCMedicion : IIdentidad<int>, IGCIdentidad, IAtencion, IAuditoria
    {
        public int Id { get; set; }

        public int RegistroId { get; set; }
                
        public string TipoConexionId { get; set; }

        public int? DistribuidoraId { get; set; }
                
        [StringLength(7)]
        public string TipoDistIdentificacionId { get; set; }
                
        [StringLength(100)]
        public string Identificacion { get; set; }
        
        [StringLength(7)]
        public string SubestacionId { get; set; }
                
        [StringLength(7)]
        public string LineaId { get; set; }

        [StringLength(500)]
        public string Circuito { get; set; }

        [Required]
        [StringLength(100)]
        public string Serie { get; set; }

        [Required]
        [StringLength(7)]
        public string TensionId { get; set; }

        public int? MedidorId { get; set; }

        public int? DocumentoId { get; set; }

        public int? UsuarioAtencionId { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? FechaAtencion { get; set; }

        public GCRegistro Registro { get; set; }

        public Catalogo TipoConexion { get; set; }

        public Agente Distribuidora { get; set; }

        public Catalogo TipoDistIdentificacion { get; set; }

        public Catalogo Subestacion { get; set; }

        public Catalogo Linea { get; set; }

        public Catalogo Tension { get; set; }

        public GCMedidorDist Medidor { get; set; }

        public GCDocumento SAS { get; set; }

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

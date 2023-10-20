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
    public class GCRegistro : IIdentidad<int>, IAuditoria, IAtencion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(7)]
        public string TipoGranClienteId { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactoNombre { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactoTelefono { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactoCorreo { get; set; }
                
        public int? ResponsableId { get; set; }

        public int? PropietarioId { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? FechaEntrada { get; set; }

        public bool FechaEditable { get; set; }

        public int? GeneralesId { get; set; }

        public int? ComercialId { get; set; }

        public int? UsuarioAtencionId { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? FechaAtencion { get; set; }

        [Required]
        [StringLength(7)]
        public string EstatusId { get; set; }

        public Catalogo TipoGranCliente { get; set; }

        public Agente Responsable { get; set; }

        public AppUser Propietario { get; set; }

        public GCGenerales Generales { get; set; }

        public GCComercial Comercial { get; set; }

        public AppUser UsuarioAtencion { get; set; }
        
        public Catalogo Estatus { get; set; }

        public ICollection<GCMedicion> Mediciones { get; set; }
        
        public ICollection<GCDocumento> Documentos { get; set; }

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

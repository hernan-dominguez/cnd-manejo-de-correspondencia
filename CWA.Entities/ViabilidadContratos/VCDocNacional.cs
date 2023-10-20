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
    public class VCDocNacional : IIdentidad<int>, IVCDocumento<VCNacional>, IAuditoria
    {
        public int Id { get; set; }

        public int RegistroId { get; set; }

        [Required]
        [StringLength(500)]
        public string Archivo { get; set; }

        [Required]
        [StringLength(7)]
        public string TipoDocumentoId { get; set; }

        public VCNacional Registro { get; set; }

        public Catalogo TipoDocumento { get; set; }

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

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

namespace CWA.Entities.Protecciones
{
    public class PROTExcelCellsEdit : IIdentidad<int>, IAuditoria
    {
        public int Id { get; set; }

        public bool Editable { get; set; }

        public string sentencia { get; set; }

        [Required]
        public string Sheet { get; set; }

        [Required]
        public int Column { get; set; }

        [Required]
        public int Row { get; set; }

        public PROTPlantilla RegPlantilla { get; set; }

        [Required]
        public int RegPlantillaId { get; set; }

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

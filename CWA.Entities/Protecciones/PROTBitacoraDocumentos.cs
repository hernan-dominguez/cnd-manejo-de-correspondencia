using CWA.Entities.Bases;
using CWA.Entities.Comun;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWA.Entities.Protecciones
{
    public class PROTBitacoraDocumentos : IIdentidad<int>, IPROTIdentidad
    {
        public int Id { get; set; }

        public int RegistroId { get; set; }

        public PROTRegistro Registro { get; set; }

        [Required]
        public string Archivo { get; set; }

        [Required]
        public string TipoDocumentoId { get; set; }

        public Catalogo TipoDocumento { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime RegFecha { get; set; }
    }
}

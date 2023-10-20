using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.ViabilidadContratos.Edit
{
    public class VCDocumentoEdit
    {
        [Required]
        public string TipoDocumentoId { get; set; }

        [Required]
        public IFormFile DocumentUpload { get; set; }
    }
}

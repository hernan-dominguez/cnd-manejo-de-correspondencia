using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.Protecciones.Edit
{
    public class PROTDocumentoEdit
    {
        [Required]
        [Display(Name = "Documento")]
        public int DocumentoId { get; set; }

        [Required]
        [Display(Name = "Archivo")]
        public IFormFile DocumentUpload { get; set; }
    }
}

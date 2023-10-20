using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.ViabilidadContratos.Edit
{
    public class VCAtencion
    {
        public int RegistroId { get; set; }

        public bool Aprobacion { get; set; }

        public string NotificacionId { get; set; }

        public string Motivo { get; set; }

        public IFormFile Adjunto { get; set; }

        public string TempPath { get; set; }

        public string MailPath { get; set; }

        public string DocsPath { get; set; }
    }
}

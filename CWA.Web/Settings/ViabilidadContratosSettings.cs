using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web.Settings.ViabilidadContratos
{
    public class ViabilidadContratosSettings
    {
        public string DocumentsTemp { get; set; }
        public string DocumentsRoot { get; set; }
        public string AttachmentsRoot { get; set; }
        public StringsSettings Strings { get; set; }
    }

    public class StringsSettings
    {
        public string Aprobar { get; set; }
        public string Rechazar { get; set; }
        public string Registrado { get; set; }
        public string Aprobado { get; set; }
        public string NoAprobado { get; set; }
        public string Pendiente { get; set; }
    }
}

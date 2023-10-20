using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web.Settings.GrandesClientes
{
    public class GrandesClientesSettings
    {
        public string DocumentsRoot { get; set; }
        public string DocumentsTemp { get; set; }
        public string TemplatesPath { get; set; }
        public StringsSettings Strings { get; set; }
    }

    public class StringsSettings
    {
        public string Programada { get; set; }
        public string NoRegistrado { get; set; }
        public string Efectiva { get; set; }
        public string Actualizado { get; set; }
        public string Aprobado { get; set; }
        public string NoAprobado { get; set; }
        public string Autorizado { get; set; }
        public string NoAutorizado { get; set; }
        public string FechaRequerido { get; set; }
        public string FechaInvalida { get; set; }
        public string ArchivoExtension { get; set; }
        public string MedidorCanales { get; set; }
        public string CanalNumero { get; set; }
        public string CanalDescripcion { get; set; }
        public string NoAprobar { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web.Settings.Protecciones
{
    public class ProteccionesSettings
    {
        public string DocumentsRoot { get; set; }

        public string DocumentsTemp { get; set; }

        public string PlantillasRoot { get; set; }

        public StringsSettings Strings { get; set; }
    }

    public class StringsSettings
    {
        public FechaEstautusSettings FechaEstatus { get; set; }

        public ItemAprobadoSettings ItemAprobado { get; set; }

        public string Autorizado { get; set; }

        public string NoAutorizado { get; set; }

        public string ArchivoExtension { get; set; }
    }

    public class FechaEstautusSettings
    {
        public string Descripcion { get; set; }

        public string Programada { get; set; }

        public string Efectiva { get; set; }

        public string Indefinida { get; set; }
    }

    public class ItemAprobadoSettings
    {
        public string Aprobado { get; set; }

        public string Pendiente { get; set; }
    }
}

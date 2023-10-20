using CWA.Web.Settings.GrandesClientes;
using CWA.Web.Settings.Protecciones;
using CWA.Web.Settings.ViabilidadContratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web.Settings
{
    public class ApplicationSettings
    {
        public string DateFormat { get; set; }
        public string DateTimeFormat { get; set; }
        public string MoneyFormat { get; set; }
        public int PageSize { get; set; }        
        public ApplicationStringsSettings Strings { get; set; }

        // Modules settings
        public GrandesClientesSettings GrandesClientes { get; set;}
        public ProteccionesSettings Protecciones { get; set; }
        public ViabilidadContratosSettings ViabilidadContratos { get; set; }
        public ManejoDeCorrespondenciaSettings ManejoDeCorrespondencia { get; set; }
    }

    public class ApplicationStringsSettings
    {
        public string GuardarDatos { get; set; }
        public string AprobarDatos { get; set; }
        public string Notificar { get; set; }
        public string ErrorHttp { get; set; }
        public string DatosInvalidos { get; set; }
        public string FechaInvalida { get; set; }
        public string PeriodoInvalido { get; set; }
        public string ArchivoExtension { get; set; }
    }
}

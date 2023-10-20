using CWA.Web.Settings.Protecciones;

namespace CWA.Web.Settings
{
    public class ManejoDeCorrespondenciaSettings
    {
        public string DocumentsTemp { get; set; }

        public StringsSettings Strings { get; set; }
    }

    public class StringsSettings
    {
        public string ExtensionInvalidaDocumentoPrincipal { get; set; }

        public string TipoDocumentoRequerido { get; set; }

        public string DireccionETESARequerida { get; set; }

        public string NotaSalienteRequerida { get; set; }

        public string LongitudMaximaNombreArchivo { get; set; }
    }
}

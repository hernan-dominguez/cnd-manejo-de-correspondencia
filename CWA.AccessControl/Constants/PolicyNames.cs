using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Constants
{
    public class PolicyNames
    {
        // Sections
        public const string AdminSectionPolicy = "ADMIN-SECTION-POLICY";
        public const string GCSectionPolicy = "GC-SECTION-POLICY";
        public const string PROTSectionPolicy = "PROT-SECTION-POLICY";
        public const string VCSectionPolicy = "VC-SECTION-POLICY";

        // Grandes Clientes
        public const string GCRegistrarPolicy = "GC-REGISTRAR-POLICY";
        public const string GCRegistroPolicy = "GC-REGISTRO-POLICY";    
        public const string GCGeneralesPolicy = "GC-GENERALES-POLICY";
        public const string GCComercialPolicy = "GC-COMERCIAL-POLICY";
        public const string GCMedicionesPolicy = "GC-MEDICIONES-POLICY";
        public const string GCMedicionPolicy = "GC-MEDICION-POLICY";
        public const string GCMedidoresDistPolicy = "GC-MEDIDORES-DIST-POLICY";
        public const string GCMedidorDistPolicy = "GC-MEDIDOR-DIST-POLICY";
        public const string GCDocumentosPolicy = "GC-DOCUMENTOS-POLICY";

        // Protecciones
        public const string PROTRegistrarPolicy = "PROT-REGISTRAR-POLICY";
        public const string PROTRegistroPolicy = "PROT-REGISTRO-POLICY";
        public const string PROTGeneralesPolicy = "PROT-GENERALES-POLICY";
        public const string PROTDocumentosPolicy = "PROT-DOCUMENTOS-POLICY";
        public const string PROTPlantillasPolicy = "PROT-PLANTILLAS-POLICY";

        // Viabilidad de contratos
        public const string VCRegionalesPolicy = "VC-REGIONALES-POLICY";
        public const string VCNacionalPolicy = "VC-NACIONAL-POLICY";
        public const string VCRegionalPolicy = "VC-REGIONAL-POLICY";
        public const string VCEnmiendaPolicy = "VC-ENMIENDA-POLICY";
    }
}

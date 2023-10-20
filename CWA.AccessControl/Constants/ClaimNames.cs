using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Constants
{
    public class ClaimNames
    {
        public const string FullName = "FULL-NAME";
        public const string UserGroup = "USER-GROUP";

        // Grandes clientes
        public const string GCSection = "GC-SECTION";
        public const string GCCreate = "GC-CREATE";
        public const string GCView = "GC-VIEW";
        public const string GCUpdate = "GC-UPDATE";
        public const string GCApprove = "GC-APPROVE";
        public const string GCEnable = "GC-ENABLE";

        //Protecciones
        public const string PROTSection = "PROT-SECTION";
        public const string PROTCreate = "PROT-CREATE";
        public const string PROTView = "PROT-VIEW";
        public const string PROTUpdate = "PROT-UPDATE";
        public const string PROTApprove = "PROT-APPROVE";
        public const string PROTEnable = "PROT-ENABLE";

        // Viabilidad de contratos
        public const string VCSection = "VC-SECTION";
        public const string VCView = "VC-VIEW";
        public const string VCUpdate = "VC-UPDATE";
        public const string VCApprove = "VC-APPROVE";

        //Manejo de correspondencia
        public const string MCSection = "MC-SECTION";
        public const string MCView = "MC-VIEW";
        public const string MCUpload = "MC-UPLOAD";
        public const string MCDownload = "MC-DOWNLOAD";
    }
}

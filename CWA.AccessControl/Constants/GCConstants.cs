using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Constants
{
    public static class GCConstants
    {
        // Authorization requirements
        public const string MEDICION_SMEC = "MEDICIONSMEC";
        public const string MEDICION_DIST = "MEDICIONDIST";

        // Page handlers
        public const string UPDATE = "UPDATE";
        public const string APPROVE = "APPROVE";
        public const string NOTIFY = "NOTIFY";
        public const string DOWNLOAD = "DOWNLOAD";
        public const string TEMPLATE = "TEMPLATE";

        // Registration status
        public const string INICIAL = "GCE01";
        public const string FECHA = "GCE02";
        public const string REQUISITOS = "GCE03";
        public const string AUTORIZADO = "GCE04";
        public const string NOTIFICADO = "GCE05";
    }
}

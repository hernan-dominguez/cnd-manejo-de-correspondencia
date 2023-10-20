﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.GrandesClientes.View
{
    public class GCDocumentoResult
    {
        public string Id { get; set; }

        public string Descripcion { get; set; }

        public string Actualizado { get; set; }

        public string Aprobado { get; set; }

        public bool AutoAccepted { get; set; }

        public DateTime SaveTime { get; set; }
    }
}

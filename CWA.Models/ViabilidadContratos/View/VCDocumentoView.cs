using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.ViabilidadContratos.View
{
    public class VCDocumentoView
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }

        public string SortOrder { get; set; }
    }
}

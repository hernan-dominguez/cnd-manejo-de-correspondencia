using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.Comun
{
    public class UserOrganization
    {
        public int? Id { get; set; }

        public string Nombre { get; set; }

        public int? AgenteId { get; set; }

        public string IdBdi { get; set; }

        public string TipoAgenteId { get; set; }
    }
}

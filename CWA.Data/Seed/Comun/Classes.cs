using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Seed.Comun
{
    public class OrganizacionAgente
    {
        public string Nombre { get; set; }
        public string IdBdi { get; set; }
        public string Codigo { get; set; }
        public string TipoAgenteId { get; set; }
        public string IdBdiResp { get; set; }
    }

    public class OrganizacionNoAgente
    {
        public string Nombre { get; set; }
    }

    public class UsuarioCnd
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string LocalPassword { get; set; }
        public string LoginId { get; set; }
        public string Group { get; set; }
    }

    public class UsuarioAgente
    {
        public string IdBdi { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Group { get; set; }
    }

    public class UsuarioNoAgente
    {
        public string Org { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Group { get; set; }
    }
}

using CWA.Entities.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Bases.ViabilidadContratos
{
    public interface IVCDocumento<T>
    { 
        public int RegistroId { get; set; }

        public string Archivo { get; set; }

        public string TipoDocumentoId { get; set; }        

        public T Registro { get; set; }

        public Catalogo TipoDocumento { get; set; }
    }
}

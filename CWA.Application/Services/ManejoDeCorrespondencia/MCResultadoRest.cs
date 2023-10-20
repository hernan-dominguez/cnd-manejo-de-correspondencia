using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Application.Services.ManejoDeCorrespondencia
{
    internal class MCResultadoRest
    {
    }

    internal class MCRaizResultadoRestArchivos
    {
        [JsonProperty("d")]
        public MCResultadosRestArchivos Result { get; set; }
    }

    internal class MCResultadosRestArchivos
    {
        [JsonProperty("results")]
        public List<MCElementoRestArchivos> ArchivosCollection { get; set; }
    }

    internal class MCElementoRestArchivos
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("TimeCreated")]
        public string TimeCreated { get; set; }

    }

    internal class MCRaizResultadoRestPropiedadesArchivo
    {
        [JsonProperty("d")]
        public MCElementoRestPropiedadesArchivo Result { get; set; }
    }

    internal class MCElementoRestPropiedadesArchivo
    {

        [JsonProperty("NúmerodeNota")]
        public string NumeroDeNota { get; set; }

    }


}

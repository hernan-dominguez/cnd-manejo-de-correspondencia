using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.ManejoDeCorrespondencia.Carga
{
    public class MCCargaDocumentos
    {
        [Display(Name = "Tipo de Documento")]
        public string TipoDocumentoId { get; set; }

        [Required]
        [Display(Name = "Nuevo Documento o Respuesta")]
        public string NuevoDocRespuesta { get; set; }

        [Display(Name = "Respuesta a Nota Saliente")]
        public string RespuestaNotaSaliente { get; set; }

        [Display(Name = "Dirección ETESA")]
        public string DireccionETESA { get; set; }

        [Required]
        [Display(Name = "Tema")]
        public string Tema { get; set; }

        [Display(Name = "Subtema")]
        public string Subtema { get; set; }

        [Required]
        [Display(Name = "Número de Nota")]
        public string NumeroDeNota { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required]
        [Display(Name = "Documento Principal")]
        public IFormFile DocumentoPrincipal { get; set; }

        [Display(Name = "Adjunto 1")]
        public IFormFile Adjunto1 { get; set; }

        [Display(Name = "Adjunto 2")]
        public IFormFile Adjunto2 { get; set; }

        [Display(Name = "Adjunto 3")]
        public IFormFile Adjunto3 { get; set; }

        [Display(Name = "Adjunto 4")]
        public IFormFile Adjunto4 { get; set; }

        [Display(Name = "Adjunto 5")]
        public IFormFile Adjunto5 { get; set; }

        [Display(Name = "Agente")]
        public string Agente { get; set; }

        [Display(Name = "Código de Usuario")]
        public string CodigoUsuario { get; set; }

        [Display(Name = "Tipo de Agente")]
        public string TipoAgente { get; set; }

        [Display(Name = "Fecha")]
        public string Fecha { get; set; }

        [Display(Name = "Sistema")]
        public string Sistema { get; set; }

        [Display(Name = "Ruta Temporal Documentos")]
        public string TempPath { get; set; }

    }
}

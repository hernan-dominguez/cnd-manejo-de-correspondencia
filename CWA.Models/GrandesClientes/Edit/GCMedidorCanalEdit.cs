using CWA.Shared.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CWA.Models.GrandesClientes.Edit
{
    public class GCMedidorCanalEdit
    {
        [Required]
        [Display(Name = "Número")]
        public int Numero { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string DescripcionId { get; set; }

        public string Display { get; set; }
    }
}
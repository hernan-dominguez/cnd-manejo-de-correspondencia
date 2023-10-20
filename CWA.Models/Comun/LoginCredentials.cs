using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Models.Comun
{
    public class LoginCredentials
    {
        [Required]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string PassWord { get; set; }

        [Display(Name = "Keep me signed in!")]
        public bool KeepSignedIn { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace RH.WEB.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "*")]
        [Display(Name = "Nombre de Usuario")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Display(Name = "Clave de Acceso")]
        public string Password { get; set; }
    }
}
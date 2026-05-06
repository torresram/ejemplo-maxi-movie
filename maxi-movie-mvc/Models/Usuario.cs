using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace maxi_movie_mvc.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public string ImagenUrlPerfil { get; set; }

        public List<Favorito>? PeliculasFavoritas { get; set; }
        public List<Review>? ReviewsUsuario { get; set; }
    }

    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo apellido es obligatorio")]
        [StringLength(50)]
        public string Apellido { get; set; }
        [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
        [Required(ErrorMessage = "El campo email es obligatorio")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Clave { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Debes confirmar la contraseña")]
        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden")]//esto asegura que el valor de ConfirmarClave sea igual al valor de Clave, lo que es común en formularios de registro para evitar errores tipográficos en la contraseña
        public string ConfirmarClave { get; set; }
    }

    public class LoginViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Correo electrónico inválido")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Clave { get; set; }
        public bool Recordarme { get; set; }
    }
}

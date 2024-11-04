using System.ComponentModel.DataAnnotations;

namespace CuentaService.DTOs
{
    public class ClienteCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder los 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El género es obligatorio")]
        [StringLength(10, ErrorMessage = "El género no debe exceder los 10 caracteres")]
        public string Genero { get; set; }

        [Range(18, 99, ErrorMessage = "La edad debe estar entre 18 y 99 años")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria")]
        public string Identificacion { get; set; }

        [StringLength(100, ErrorMessage = "La dirección no debe exceder los 100 caracteres")]
        public string Direccion { get; set; }

        [Phone(ErrorMessage = "El teléfono no tiene un formato válido")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 20 caracteres")]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string Estado { get; set; }
    }


}

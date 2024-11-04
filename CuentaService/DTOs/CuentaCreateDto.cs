using System.ComponentModel.DataAnnotations;

namespace CuentaService.DTOs
{
    public class CuentaCreateDto
    {
        [Required(ErrorMessage = "El número de cuenta es obligatorio")]
        public string NumeroCuenta { get; set; }

        [Required(ErrorMessage = "El tipo de cuenta es obligatorio")]
        public string TipoCuenta { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El saldo inicial debe ser positivo")]
        public decimal SaldoInicial { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "El ID del cliente es obligatorio")]
        public string ClienteId { get; set; }
    }


}

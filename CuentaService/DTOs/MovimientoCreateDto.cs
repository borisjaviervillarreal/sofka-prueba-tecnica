using System.ComponentModel.DataAnnotations;

namespace CuentaService.DTOs
{
    public class MovimientoCreateDto
    {
        [Required(ErrorMessage = "El tipo de movimiento es obligatorio")]
        [RegularExpression("Deposito|Retiro", ErrorMessage = "El tipo de movimiento debe ser 'Deposito' o 'Retiro'")]
        public string TipoMovimiento { get; set; }  //Deposito o Retiro

        [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser mayor a cero")]
        public decimal Valor { get; set; }
    }

}

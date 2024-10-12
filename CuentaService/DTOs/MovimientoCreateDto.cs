namespace CuentaService.DTOs
{
    public class MovimientoCreateDto
    {
        public string TipoMovimiento { get; set; }  // "Deposito" o "Retiro"
        public decimal Valor { get; set; }
    }


}

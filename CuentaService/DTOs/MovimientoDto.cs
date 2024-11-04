namespace CuentaService.DTOs
{
    public class MovimientoDto
    {
        public int Id { get; init; }
        public DateTime Fecha { get; init; }
        public string TipoMovimiento { get; init; }
        public decimal Valor { get; init; }
        public decimal Saldo { get; init; }
        public int CuentaId { get; init; }
    }

}

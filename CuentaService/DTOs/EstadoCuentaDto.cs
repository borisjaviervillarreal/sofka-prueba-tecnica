namespace CuentaService.DTOs
{
    public class EstadoCuentaDto
    {
        public string Fecha { get; init; }
        public string Cliente { get; init; }
        public string Identificacion { get; init; }
        public string NumeroCuenta { get; init; }
        public string Tipo { get; init; }
        public decimal SaldoInicial { get; init; }
        public bool Estado { get; init; }
        public decimal Movimiento { get; init; }
        public decimal SaldoDisponible { get; init; }
    }

}

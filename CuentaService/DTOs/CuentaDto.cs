namespace CuentaService.DTOs
{
    public class CuentaDto
    {
        public int Id { get; init; }
        public string NumeroCuenta { get; init; }
        public string TipoCuenta { get; init; }
        public decimal SaldoInicial { get; init; }
        public string Estado { get; init; }
        public string ClienteId { get; init; }
    }

}

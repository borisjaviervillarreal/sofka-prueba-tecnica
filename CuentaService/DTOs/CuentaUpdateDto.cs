namespace CuentaService.DTOs
{
    public class CuentaUpdateDto
    {
        public string NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; }
        public decimal SaldoInicial { get; set; }
        public string Estado { get; set; }
        public string ClienteId { get; set; }
    }

}

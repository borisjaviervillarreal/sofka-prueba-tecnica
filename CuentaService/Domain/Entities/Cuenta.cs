namespace CuentaService.Domain.Entities
{
    public class Cuenta
    {
        public int Id { get; set; }  
        public string NumeroCuenta { get; set; }  
        public string TipoCuenta { get; set; }  
        public decimal SaldoInicial { get; set; }
        public string Estado { get; set; }
        // Relación con el cliente
        public int ClienteId { get; set; }
    }
}

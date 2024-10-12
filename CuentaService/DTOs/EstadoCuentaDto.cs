namespace CuentaService.DTOs
{
    namespace CuentaService.DTOs
    {
        public class EstadoCuentaDto
        {
            public string Fecha { get; set; }
            public string Cliente { get; set; }
            public string Identificacion { get; set; }
            public string NumeroCuenta { get; set; }
            public string Tipo { get; set; }
            public decimal SaldoInicial { get; set; }
            public bool Estado { get; set; }
            public decimal Movimiento { get; set; }
            public decimal SaldoDisponible { get; set; }
        }
    }

}

namespace CuentaService.Application.Exceptions
{
    public class SaldoInsuficienteException : AppException
    {
        public SaldoInsuficienteException(decimal saldoActual, decimal valorMovimiento)
            : base($"Saldo insuficiente. Saldo actual: {saldoActual}. Intento de movimiento: {valorMovimiento}.", 400) { }
    }
}

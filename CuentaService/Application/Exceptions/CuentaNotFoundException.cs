namespace CuentaService.Application.Exceptions
{
    public class CuentaNotFoundException : AppException
    {
        public CuentaNotFoundException(int cuentaId)
            : base($"La cuenta con ID {cuentaId} no fue encontrada.", 404) { }
    }
}

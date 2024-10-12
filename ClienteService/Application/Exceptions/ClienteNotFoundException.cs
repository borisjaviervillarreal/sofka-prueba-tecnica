namespace ClienteService.Application.Exceptions
{
    public class ClienteNotFoundException : AppException
    {
        public ClienteNotFoundException(string clienteId)
            : base($"El cliente con ID {clienteId} no fue encontrado.", 404) { }
    }
}

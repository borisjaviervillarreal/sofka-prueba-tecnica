using ClienteService.DTOs;

namespace ClienteService.Producers.RabbitMQ
{
    public interface IClienteCreatedPublisher
    {
        void PublishCliente(ClienteInfoDto clienteInfo);
    }
}

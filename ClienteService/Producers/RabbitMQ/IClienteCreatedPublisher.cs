using ClienteService.Domain.Events;
using ClienteService.DTOs;

namespace ClienteService.Producers.RabbitMQ
{
    public interface IClienteCreatedPublisher
    {
        void PublishClienteCreated(ClienteCreatedEvent clienteCreatedEvent);
        void PublishClienteUpdated(ClienteUpdatedEvent clienteUpdatedEvent);
    }
}

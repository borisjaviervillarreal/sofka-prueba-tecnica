using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using ClienteService.DTOs;
using RabbitMQ.Client.Exceptions;

namespace ClienteService.Producers.RabbitMQ
{
    public class ClienteCreatedPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public ClienteCreatedPublisher()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: "cliente_queue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine("No se pudo conectar a RabbitMQ. Verificar que el contenedor esté levantado: " + ex.Message);
                throw;
            }
        }
    }
}

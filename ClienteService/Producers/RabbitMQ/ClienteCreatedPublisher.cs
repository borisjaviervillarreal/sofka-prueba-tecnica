using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using ClienteService.DTOs;
using RabbitMQ.Client.Exceptions;

namespace ClienteService.Producers.RabbitMQ
{
    public class ClienteCreatedPublisher : IClienteCreatedPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public ClienteCreatedPublisher(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:Host"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declarar el exchange
                _channel.ExchangeDeclare(exchange: "cliente_exchange",
                                         type: "direct",
                                         durable: true,
                                         autoDelete: false,
                                         arguments: null);

                // Declarar la cola
                _channel.QueueDeclare(queue: "cliente_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Vincular la cola al exchange
                _channel.QueueBind(queue: "cliente_queue",
                                   exchange: "cliente_exchange",
                                   routingKey: "cliente_routing_key");
            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine("No se pudo conectar a RabbitMQ. Verificar que el contenedor esté levantado: " + ex.Message);
                throw;
            }
        }


        public void PublishCliente(ClienteInfoDto clienteInfo)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clienteInfo));
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(exchange: "cliente_exchange",
                                      routingKey: "cliente_routing_key",
                                      basicProperties: properties,
                                      body: body);

                Console.WriteLine($"Mensaje publicado: Cliente {clienteInfo.Nombre} con ID {clienteInfo.ClienteId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al publicar el mensaje a RabbitMQ: {ex.Message}");
            }
        }

    }
}

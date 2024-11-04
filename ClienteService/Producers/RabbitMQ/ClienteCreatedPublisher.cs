using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using ClienteService.DTOs;
using RabbitMQ.Client.Exceptions;
using Polly;

namespace ClienteService.Producers.RabbitMQ
{
    public class ClienteCreatedPublisher : IClienteCreatedPublisher
    {
        private IConnection _connection;
        private IModel _channel;

        public ClienteCreatedPublisher(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:Host"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            // Definir Circuit Breaker y la política de reintento para mayor resiliencia
            var circuitBreakerPolicy = Policy
                .Handle<BrokerUnreachableException>()
                .CircuitBreaker(2, TimeSpan.FromSeconds(30),
                    onBreak: (exception, timespan) =>
                    {
                        Console.WriteLine("Circuito abierto: el servicio no está disponible.");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("Circuito cerrado: el servicio está disponible nuevamente.");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine("Circuito en semiabierto: probando el servicio.");
                    });

            var retryPolicy = Policy
                .Handle<BrokerUnreachableException>()
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Intento {retryCount} fallido de conexión. Reintentando en {timeSpan.Seconds} segundos.");
                    });

            // Ejcutar la conexión con el Circuit Breaker y la política de reintento
            circuitBreakerPolicy.Execute(() =>
            {
                retryPolicy.Execute(() =>
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    _channel.ExchangeDeclare(exchange: "cliente_exchange", type: "direct", durable: true, autoDelete: false, arguments: null);
                    _channel.QueueDeclare(queue: "cliente_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    _channel.QueueBind(queue: "cliente_queue", exchange: "cliente_exchange", routingKey: "cliente_routing_key");
                });
            });
        }

        public void PublishCliente(ClienteInfoDto clienteInfo)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clienteInfo));
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            // Política de reintento para la publicación de mensajes
            var publishRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromMilliseconds(200 * retryAttempt),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Fallo al publicar mensaje. Intento {retryCount} en {timeSpan.TotalMilliseconds} ms.");
                    });

            publishRetryPolicy.Execute(() =>
            {
                _channel.BasicPublish(exchange: "cliente_exchange",
                                      routingKey: "cliente_routing_key",
                                      basicProperties: properties,
                                      body: body);

                Console.WriteLine($"Mensaje publicado: Cliente {clienteInfo.Nombre} con ID {clienteInfo.ClienteId}");
            });
        }

    }
}

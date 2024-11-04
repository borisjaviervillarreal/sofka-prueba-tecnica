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
        private readonly ILogger<ClienteCreatedPublisher> _logger;

        public ClienteCreatedPublisher(IConfiguration configuration, ILogger<ClienteCreatedPublisher> logger)
        {
            _logger = logger;
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
                        _logger.LogError("Circuito abierto: el servicio de RabbitMQ no está disponible.");
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuito cerrado: el servicio de RabbitMQ está disponible nuevamente.");
                    },
                    onHalfOpen: () =>
                    {
                        _logger.LogInformation("Circuito en semiabierto: probando el servicio de RabbitMQ.");
                    });

            var retryPolicy = Policy
                .Handle<BrokerUnreachableException>()
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Intento {retryCount} fallido de conexión. Reintentando en {timeSpan.Seconds} segundos.");
                    });

            // Ejcutar la conexión con el Circuit Breaker y la política de reintento
            circuitBreakerPolicy.Execute(() =>
            {
                retryPolicy.Execute(() =>
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _channel.ConfirmSelect(); // Activar el modo de confirmación

                    _channel.ExchangeDeclare(exchange: "cliente_exchange", type: "direct", durable: true, autoDelete: false, arguments: null);
                    _channel.QueueDeclare(queue: "cliente_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    _channel.QueueBind(queue: "cliente_queue", exchange: "cliente_exchange", routingKey: "cliente_routing_key");

                    _logger.LogInformation("Conexión a RabbitMQ establecida y configurada con éxito.");
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
                        _logger.LogWarning($"Fallo al publicar mensaje. Intento {retryCount} en {timeSpan.TotalMilliseconds} ms.");
                    });

            publishRetryPolicy.Execute(() =>
            {
                _channel.BasicPublish(exchange: "cliente_exchange",
                                      routingKey: "cliente_routing_key",
                                      basicProperties: properties,
                                      body: body);

                // Esperar confirmación de RabbitMQ
                if (!_channel.WaitForConfirms())
                {
                    _logger.LogInformation($"Mensaje publicado y confirmado: Cliente {clienteInfo.Nombre} con ID {clienteInfo.ClienteId}");
                }
                else
                {
                    _logger.LogError("La confirmación del mensaje falló.");
                    throw new Exception("La confirmación del mensaje falló.");
                }

            });
        }

    }
}

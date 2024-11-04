using ClienteService.Domain.Events;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace ClienteService.Producers.RabbitMQ
{
    public class ClienteCreatedPublisher : IClienteCreatedPublisher
    {
        private readonly IModel _channel;
        private readonly ILogger<ClienteCreatedPublisher> _logger;

        public ClienteCreatedPublisher(IConnection connection, ILogger<ClienteCreatedPublisher> logger)
        {
            _logger = logger;
            _channel = connection.CreateModel();

            // Configuración del exchange y cola
            _channel.ExchangeDeclare(exchange: "cliente_exchange", type: "direct", durable: true, autoDelete: false);
            _channel.QueueDeclare(queue: "cliente_queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: "cliente_queue", exchange: "cliente_exchange", routingKey: "cliente_routing_key");
        }

        public void PublishClienteCreated(ClienteCreatedEvent clienteCreatedEvent)
        {
            PublishWithResilience(clienteCreatedEvent);
        }

        public void PublishClienteUpdated(ClienteUpdatedEvent clienteUpdatedEvent)
        {
            PublishWithResilience(clienteUpdatedEvent);
        }

        private void PublishWithResilience<T>(T clienteEvent)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clienteEvent));
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

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

            // Política de reintento con timeout para la publicación de mensajes
            var publishRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromMilliseconds(200 * retryAttempt),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Fallo al publicar mensaje. Intento {retryCount} en {timeSpan.TotalMilliseconds} ms.");
                    });

            // Ejecutar la publicación con Circuit Breaker, Retry y Timeout
            circuitBreakerPolicy.Execute(() =>
            {
                retryPolicy.Execute(() =>
                {
                    publishRetryPolicy.Execute(() =>
                    {
                        _channel.BasicPublish(exchange: "cliente_exchange",
                                              routingKey: "cliente_routing_key",
                                              basicProperties: properties,
                                              body: body);

                        if (_channel.WaitForConfirms())
                        {
                            _logger.LogInformation($"Mensaje publicado y confirmado: Evento de Cliente {clienteEvent.GetType().Name}");
                        }
                        else
                        {
                            _logger.LogError("La confirmación del mensaje falló.");
                            throw new Exception("La confirmación del mensaje falló.");
                        }
                    });
                });
            });
        }
    }
}

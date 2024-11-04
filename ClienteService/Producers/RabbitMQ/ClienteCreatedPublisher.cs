using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using ClienteService.DTOs;
using RabbitMQ.Client.Exceptions;
using Polly;
using ClienteService.Domain.Events;

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
            PublishEvent(clienteCreatedEvent);
        }

        public void PublishClienteUpdated(ClienteUpdatedEvent clienteUpdatedEvent)
        {
            PublishEvent(clienteUpdatedEvent);
        }
        private void PublishEvent<T>(T clienteEvent)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clienteEvent));
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

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

                if (!_channel.WaitForConfirms())
                {
                    _logger.LogInformation($"Mensaje publicado y confirmado: {typeof(T).Name}");
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


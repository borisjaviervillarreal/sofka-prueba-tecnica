using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using CuentaService.Domain.Events;

namespace CuentaService.Producers.RabbitMQ
{
    public class ClienteCreatedConsumer : BackgroundService
    {
        private readonly IModel _channel;
        private readonly ILogger<ClienteCreatedConsumer> _logger;

        public static ConcurrentDictionary<string, ClienteCreatedEvent> ClientesInfo = new ConcurrentDictionary<string, ClienteCreatedEvent>();

        public ClienteCreatedConsumer(IConnection connection, ILogger<ClienteCreatedConsumer> logger)
        {
            _logger = logger;
            _channel = connection.CreateModel();

            // Configuración del exchange y cola
            _channel.ExchangeDeclare(exchange: "cliente_exchange", type: "direct", durable: true, autoDelete: false);
            _channel.QueueDeclare(queue: "cliente_queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: "cliente_queue", exchange: "cliente_exchange", routingKey: "cliente_routing_key");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var clienteEvent = JsonSerializer.Deserialize<ClienteCreatedEvent>(message);

                    if (clienteEvent != null)
                    {
                        ClientesInfo[clienteEvent.ClienteId] = clienteEvent;
                        _logger.LogInformation($"Cliente recibido: {clienteEvent.Nombre} ({clienteEvent.ClienteId})");

                        _channel.BasicAck(ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        _logger.LogWarning("El mensaje recibido no es válido.");
                        _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al procesar el mensaje: {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: "cliente_queue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}


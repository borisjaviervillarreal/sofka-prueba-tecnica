using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using CuentaService.DTOs;
using System.Collections.Concurrent;
using RabbitMQ.Client.Exceptions;

namespace CuentaService.Producers.RabbitMQ
{
    public class ClienteCreatedConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        // Almacenamiento en memoria para los datos del cliente
        public static ConcurrentDictionary<string, ClienteInfoDto> ClientesInfo = new ConcurrentDictionary<string, ClienteInfoDto>();

        public ClienteCreatedConsumer(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:Host"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            int retryCount = 0;
            bool connected = false;
            while (!connected && retryCount < 10)
            {
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

                    connected = true; // Conexión exitosa
                }
                catch (Exception ex)
                {
                    retryCount++;
                    Console.WriteLine($"Intentando reconectar a RabbitMQ... ({retryCount})");
                    Thread.Sleep(5000);
                }
            }

            if (!connected)
            {
                throw new Exception("No se pudo conectar a RabbitMQ después de varios intentos.");
            }
        }

        public void StartConsuming()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var clienteInfo = JsonSerializer.Deserialize<ClienteInfoDto>(message);

                // Almacenar la información del cliente en el diccionario
                if (clienteInfo != null)
                {
                    ClientesInfo[clienteInfo.ClienteId] = clienteInfo;
                    Console.WriteLine($"Cliente recibido: {clienteInfo.Nombre} ({clienteInfo.ClienteId})");

                    // Confirmar la recepción del mensaje
                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                else
                {
                    Console.WriteLine("El mensaje recibido del cliente no es válido.");
                }
            };

            _channel.BasicConsume(queue: "cliente_queue",
                                 autoAck: false,
                                 consumer: consumer);
        }
    }

}

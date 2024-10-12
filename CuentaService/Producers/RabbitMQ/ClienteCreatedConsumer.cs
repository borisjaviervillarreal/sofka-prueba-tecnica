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
        public static ConcurrentDictionary<int, ClienteInfoDto> ClientesInfo = new ConcurrentDictionary<int, ClienteInfoDto>();

        public ClienteCreatedConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };

            int retryCount = 0;
            bool connected = false;
            while (!connected && retryCount < 10) 
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _channel.QueueDeclare(queue: "cliente_queue",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
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
                    ClientesInfo[clienteInfo.Id] = clienteInfo;
                    Console.WriteLine($"Cliente recibido: {clienteInfo.Nombre} ({clienteInfo.Identificacion})");
                }
                else
                {
                    Console.WriteLine("El mensaje recibido del cliente no es válido.");
                }
            };

            _channel.BasicConsume(queue: "cliente_queue",
                                 autoAck: true,
                                 consumer: consumer);
        }
    }

}

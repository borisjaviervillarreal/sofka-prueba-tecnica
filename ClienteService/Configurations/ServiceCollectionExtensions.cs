using ClienteService.Application.Exceptions;
using ClienteService.Application.Services;
using ClienteService.Domain.Interfaces;
using ClienteService.Infrastructure.Data;
using ClienteService.Infrastructure.Repositories;
using ClienteService.Producers.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace ClienteService.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuración del DbContext
            services.AddDbContext<ClienteDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ClienteDatabase")));

            // Configuración del filtro de Excepciones Global
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
                options.Filters.Add<GlobalExceptionFilter>();
            });

            // Configuración de RabbitMQ como Singleton
            services.AddSingleton<IConnection>(provider =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMQ:Host"],
                    Port = int.Parse(configuration["RabbitMQ:Port"]),
                    UserName = configuration["RabbitMQ:UserName"],
                    Password = configuration["RabbitMQ:Password"]
                };
                return factory.CreateConnection();
            });

            // Inyectar el publicador de eventos
            services.AddSingleton<IClienteCreatedPublisher, ClienteCreatedPublisher>();

            // Inyectar Servicios
            services.AddScoped<IClienteService, ClientService>();
            services.AddScoped<IClienteRepository, ClienteRepository>();

            // Configuración de AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}


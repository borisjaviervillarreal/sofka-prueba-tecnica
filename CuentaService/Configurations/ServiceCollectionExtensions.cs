using CuentaService.Application.Exceptions;
using CuentaService.Application.Services;
using CuentaService.Domain.Interfaces;
using CuentaService.Infrastructure.Data;
using CuentaService.Infrastructure.Repositories;
using CuentaService.Producers.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace CuentaService.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuración del DbContext
            services.AddDbContext<CuentaDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CuentaDatabase")));

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

            // Registrar ClienteCreatedConsumer como HostedService
            //services.AddHostedService<ClienteCreatedConsumer>();

            // Inyectar Servicios y Repositorios
            services.AddScoped<ICuentaService, CuentService>();
            services.AddScoped<ICuentaRepository, CuentaRepository>();
            services.AddScoped<IMovimientoRepository, MovimientoRepository>();

            // Configuración de AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}

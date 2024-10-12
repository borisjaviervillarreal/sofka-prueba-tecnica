using ClienteService.Application.Services;
using ClienteService.Domain.Interfaces;
using ClienteService.Infrastructure.Data;
using ClienteService.Infrastructure.Repositories;
using ClienteService.Producers.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClienteService.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            
            // Configuración del DbContext
            services.AddDbContext<ClienteDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ClienteDatabase")));

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

using CuentaService.Application.Services;
using CuentaService.Domain.Interfaces;
using CuentaService.Infrastructure.Data;
using CuentaService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CuentaService.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuración del DbContext usando la cadena de conexión proporcionada en la configuración
            services.AddDbContext<CuentaDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CuentaDatabase")));

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

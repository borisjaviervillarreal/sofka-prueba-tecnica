using ClienteService.Application.Middleware;
using ClienteService.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders(); // Limpia proveedores predeterminados
builder.Logging.AddConsole(); // Agrega la salida de log a la consola
builder.Logging.SetMinimumLevel(LogLevel.Error); // Configura el nivel mínimo de logging a Error
// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();


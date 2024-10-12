using CuentaService.DTOs;
using CuentaService.DTOs.CuentaService.DTOs;

namespace CuentaService.Application.Services
{
    public interface ICuentaService
    {
        Task<IEnumerable<CuentaDto>> GetAllCuentasAsync();
        Task<CuentaDto> GetCuentaByIdAsync(int id);
        Task AddCuentaAsync(CuentaDto cuentaDto);
        Task UpdateCuentaAsync(CuentaDto cuentaDto);
        Task DeleteCuentaAsync(int id);

        Task<IEnumerable<MovimientoDto>> GetMovimientosByCuentaIdAsync(int cuentaId);
        Task AddMovimientoAsync(MovimientoDto movimientoDto);
        Task<List<EstadoCuentaDto>> GetEstadoCuentaAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin);
    }
}

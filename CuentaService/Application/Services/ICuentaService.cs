using CuentaService.DTOs;
using CuentaService.DTOs.CuentaService.DTOs;

namespace CuentaService.Application.Services
{
    public interface ICuentaService
    {
        Task<IEnumerable<CuentaDto>> GetAllCuentasAsync();
        Task<CuentaDto> GetCuentaByIdAsync(int id);
        Task<CuentaDto> AddCuentaAsync(CuentaCreateDto cuentaDto);
        Task UpdateCuentaAsync(int id, CuentaUpdateDto cuentaUpdateDto);
        Task DeleteCuentaAsync(int id);

        Task<IEnumerable<MovimientoDto>> GetMovimientosByCuentaIdAsync(int cuentaId);
        Task AddMovimientoAsync(int cuentaId, MovimientoCreateDto movimientoDto);
        Task<List<EstadoCuentaDto>> GetEstadoCuentaAsync(string clienteId, DateTime fechaInicio, DateTime fechaFin);
    }
}

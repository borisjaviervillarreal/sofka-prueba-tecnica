using CuentaService.Domain.Entities;

namespace CuentaService.Domain.Interfaces
{
    public interface IMovimientoRepository
    {
        Task<IEnumerable<Movimiento>> GetAllMovimientosAsync();
        Task<IEnumerable<Movimiento>> GetMovimientosByCuentaIdAsync(int cuentaId);
        Task AddMovimientoAsync(Movimiento movimiento);
    }
}

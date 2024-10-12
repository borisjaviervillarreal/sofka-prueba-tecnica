using CuentaService.Domain.Entities;

namespace CuentaService.Domain.Interfaces
{
    public interface ICuentaRepository
    {
        Task<IEnumerable<Cuenta>> GetAllCuentasAsync();
        Task<Cuenta> GetCuentaByIdAsync(int id);
        Task AddCuentaAsync(Cuenta cuenta);
        Task UpdateCuentaAsync(Cuenta cuenta);
        Task DeleteCuentaAsync(int id);
    }
}

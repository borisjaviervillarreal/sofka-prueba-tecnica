using CuentaService.Application.Exceptions;
using CuentaService.Domain.Entities;
using CuentaService.Domain.Interfaces;
using CuentaService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CuentaService.Infrastructure.Repositories
{
    public class CuentaRepository : ICuentaRepository
    {
        private readonly CuentaDbContext _context;

        public CuentaRepository(CuentaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Cuenta>> GetAllCuentasAsync()
        {
            return await _context.Cuentas.ToListAsync();
        }

        public async Task<Cuenta> GetCuentaByIdAsync(int id)
        {
            return await _context.Cuentas.FindAsync(id);
        }

        public async Task AddCuentaAsync(Cuenta cuenta)
        {
            await _context.Cuentas.AddAsync(cuenta);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCuentaAsync(Cuenta cuenta)
        {
            _context.Cuentas.Update(cuenta);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCuentaAsync(int id)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta != null)
            {
                cuenta.Estado = "Inactiva"; // Eliminado lógico cambiando el estado
                _context.Cuentas.Update(cuenta);
                await _context.SaveChangesAsync();
            }
        }
    }
}

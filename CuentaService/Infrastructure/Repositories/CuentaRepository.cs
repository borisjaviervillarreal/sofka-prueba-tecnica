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
            _context = context;
        }

        public async Task<IEnumerable<Cuenta>> GetAllCuentasAsync()
        {
            try
            {
                return await _context.Cuentas.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("Error al obtener la lista de cuentas desde la base de datos.", 500);
            }
        }

        public async Task<Cuenta> GetCuentaByIdAsync(int id)
        {
            try
            {
                return await _context.Cuentas.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new AppException("Error al obtener información de la cuenta desde la base de datos.", 500);
            }

        }

        public async Task AddCuentaAsync(Cuenta cuenta)
        {
            try
            {
                await _context.Cuentas.AddAsync(cuenta);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("Error al agregar cuenta a la base de datos.", 500);
            }

        }

        public async Task UpdateCuentaAsync(Cuenta cuenta)
        {
            try
            {
                _context.Cuentas.Update(cuenta);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("Error al actualizar cuenta a la base de datos.", 500);
            }

        }

        public async Task DeleteCuentaAsync(int id)
        {
            try
            {
                var cuenta = await _context.Cuentas.FindAsync(id);
                if (cuenta != null)
                {
                    cuenta.Estado = "Inactiva";
                    _context.Cuentas.Update(cuenta);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new AppException("Error al eliminar la cuenta de la base de datos.", 500);
            }
        }
    }
}

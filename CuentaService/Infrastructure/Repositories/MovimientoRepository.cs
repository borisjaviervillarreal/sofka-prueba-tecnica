using CuentaService.Application.Exceptions;
using CuentaService.Domain.Entities;
using CuentaService.Domain.Interfaces;
using CuentaService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CuentaService.Infrastructure.Repositories
{
    public class MovimientoRepository : IMovimientoRepository
    {
        private readonly CuentaDbContext _context;

        public MovimientoRepository(CuentaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movimiento>> GetAllMovimientosAsync()
        {
            try
            {
                return await _context.Movimientos.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("Error al consultar los movimientos de la base de datos.", 500);
            }

        }

        public async Task<IEnumerable<Movimiento>> GetMovimientosByCuentaIdAsync(int cuentaId)
        {
            try
            {
                return await _context.Movimientos
                                 .Where(m => m.CuentaId == cuentaId)
                                 .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("Error al consultar los movimientos de cuenta de la base de datos.", 500);
            }
        }

        public async Task AddMovimientoAsync(Movimiento movimiento)
        {
            try
            {
                await _context.Movimientos.AddAsync(movimiento);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("Error al agregar los movimientos a la base de datos.", 500);
            }
            
        }
    }
}

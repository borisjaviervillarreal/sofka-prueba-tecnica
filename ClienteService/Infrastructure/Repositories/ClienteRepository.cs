using ClienteService.Application.Exceptions;
using ClienteService.Domain.Entities;
using ClienteService.Domain.Interfaces;
using ClienteService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClienteService.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ClienteDbContext _context;

        public ClienteRepository(ClienteDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            try
            {
                return await _context.Clientes.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("Error al obtener la lista de clientes desde la base de datos.", 500);
            }
        }

        public async Task<Cliente> GetClienteByIdAsync(string clienteId)
        {
            try
            {
                return await _context.Clientes.SingleOrDefaultAsync(c => c.ClienteId == clienteId);
            }
            catch (Exception ex)
            {
                throw new AppException("Error al obtener el cliente desde la base de datos.", 500);
            }
        }


        public async Task AddClienteAsync(Cliente cliente)
        {
            try
            {
                await _context.Clientes.AddAsync(cliente);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("Error al agregar el cliente a la base de datos.", 500);
            }
        }

        public async Task UpdateClienteAsync(Cliente cliente)
        {
            try
            {
                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException("Error al actualizar el cliente a la base de datos.", 500);
            }

        }

        public async Task DeleteClienteAsync(string clienteId)
        {
            try
            {
                var cliente = await _context.Clientes.SingleOrDefaultAsync(c => c.ClienteId == clienteId);
                if (cliente == null)
                {
                    throw new ClienteNotFoundException(clienteId);
                }

                cliente.Estado = "Inactivo"; // Eliminado lógico cambiando el estado
                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
            }
            catch (ClienteNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException("Error al eliminar el cliente de la base de datos.", 500);
            }
        }

    }
}

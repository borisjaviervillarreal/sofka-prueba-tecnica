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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente> GetClienteByIdAsync(string clienteId)
        {
            return await _context.Clientes.SingleOrDefaultAsync(c => c.ClienteId == clienteId);
        }

        public async Task AddClienteAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClienteAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClienteAsync(string clienteId)
        {
            var cliente = await _context.Clientes.SingleOrDefaultAsync(c => c.ClienteId == clienteId);
            if (cliente == null)
            {
                throw new ClienteNotFoundException(clienteId); // Excepción personalizada para cliente no encontrado
            }

            cliente.Estado = "Inactivo"; // Eliminado lógico cambiando el estado
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

    }
}

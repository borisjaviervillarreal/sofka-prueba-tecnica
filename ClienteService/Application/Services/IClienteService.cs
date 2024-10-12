using ClienteService.DTOs;

namespace ClienteService.Application.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> GetAllClientesAsync();
        Task<ClienteDto> GetClienteByIdAsync(int id);
        Task AddClienteAsync(ClienteDto clienteDto);
        Task UpdateClienteAsync(ClienteDto clienteDto);
        Task DeleteClienteAsync(int id);
    }
}

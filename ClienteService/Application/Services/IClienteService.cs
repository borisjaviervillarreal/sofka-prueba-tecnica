using ClienteService.Domain.Entities;
using ClienteService.DTOs;
using CuentaService.DTOs;

namespace ClienteService.Application.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> GetAllClientesAsync();
        Task<ClienteDto> GetClienteByIdAsync(string clienteId);
        Task<ClienteCreatedDto> AddClienteAsync(ClienteCreateDto clienteDto);
        Task UpdateClienteAsync(string clienteId, ClienteUpdateDto clienteDto);
        Task DeleteClienteAsync(string clienteId);
    }
}

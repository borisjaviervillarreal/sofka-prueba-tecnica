using AutoMapper;
using ClienteService.Application.Exceptions;
using ClienteService.Domain.Entities;
using ClienteService.Domain.Interfaces;
using ClienteService.DTOs;
using ClienteService.Producers.RabbitMQ;

namespace ClienteService.Application.Services
{
    public class ClientService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public ClientService(IClienteRepository clienteRepository, IMapper mapper)
        {
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ClienteDto>> GetAllClientesAsync()
        {
            try
            {
                var clientes = await _clienteRepository.GetAllClientesAsync();
                return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener la lista de clientes: {ex.Message}", 500);
            }
        }

        public async Task<ClienteDto> GetClienteByIdAsync(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetClienteByIdAsync(id);
                if (cliente == null)
                {
                    throw new ClienteNotFoundException(id);
                }

                return _mapper.Map<ClienteDto>(cliente);
            }
            catch (ClienteNotFoundException) 
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener el cliente con ID {id}: {ex.Message}", 500);
            }
        }


        public async Task AddClienteAsync(ClienteDto clienteDto)
        {
            if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));

            try
            {
                var cliente = _mapper.Map<Cliente>(clienteDto);
                await _clienteRepository.AddClienteAsync(cliente);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al agregar el cliente: {ex.Message}", 500);
            }
        }

        public async Task UpdateClienteAsync(ClienteDto clienteDto)
        {
            if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));

            try
            {
                var cliente = _mapper.Map<Cliente>(clienteDto);
                await _clienteRepository.UpdateClienteAsync(cliente);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al actualizar el cliente con ID {clienteDto.Id}: {ex.Message}", 500);
            }
        }

        public async Task DeleteClienteAsync(int id)
        {
            try
            {
                await _clienteRepository.DeleteClienteAsync(id);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al eliminar el cliente con ID {id}: {ex.Message}", 500);
            }
        }
    }

}


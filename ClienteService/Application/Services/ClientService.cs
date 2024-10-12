using AutoMapper;
using ClienteService.Application.Exceptions;
using ClienteService.Domain.Entities;
using ClienteService.Domain.Interfaces;
using ClienteService.DTOs;
using ClienteService.Producers.RabbitMQ;
using CuentaService.DTOs;

namespace ClienteService.Application.Services
{
    public class ClientService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;
        private readonly IClienteCreatedPublisher _clientePublisher;
        public ClientService(IClienteRepository clienteRepository, IMapper mapper, IClienteCreatedPublisher clientePublisher)
        {
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _clientePublisher = clientePublisher;
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

        public async Task<ClienteDto> GetClienteByIdAsync(string clienteId)
        {
            try
            {
                var cliente = await _clienteRepository.GetClienteByIdAsync(clienteId);
                if (cliente == null)
                {
                    throw new ClienteNotFoundException(clienteId);
                }

                return _mapper.Map<ClienteDto>(cliente);
            }
            catch (ClienteNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener el cliente con ClienteId {clienteId}: {ex.Message}", 500);
            }
        }



        public async Task<Cliente> AddClienteAsync(ClienteCreateDto clienteDto)
        {
            if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));

            try
            {
                var cliente = _mapper.Map<Cliente>(clienteDto);
                // Generar ClienteId automáticamente
                if (string.IsNullOrEmpty(cliente.ClienteId))
                {
                    cliente.ClienteId = Guid.NewGuid().ToString();
                }
                await _clienteRepository.AddClienteAsync(cliente);

                // Publicar cliente creado en RabbitMQ
                var clienteInfo = _mapper.Map<ClienteInfoDto>(cliente);
                _clientePublisher.PublishCliente(clienteInfo);

                return cliente;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al agregar el cliente: {ex.Message}", 500);
            }
        }

        public async Task UpdateClienteAsync(string clienteId, ClienteUpdateDto clienteDto)
        {
            if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));

            try
            {
                var cliente = await _clienteRepository.GetClienteByIdAsync(clienteId);
                if (cliente == null)
                {
                    throw new ClienteNotFoundException($"El cliente con ID {clienteId} no fue encontrado.");
                }

                _mapper.Map(clienteDto, cliente);

                await _clienteRepository.UpdateClienteAsync(cliente);

                // Publicar cliente actualizado en RabbitMQ
                var clienteInfo = _mapper.Map<ClienteInfoDto>(cliente);
                _clientePublisher.PublishCliente(clienteInfo);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al actualizar el cliente con ClienteId {clienteId}: {ex.Message}", 500);
            }
        }



        public async Task DeleteClienteAsync(string clienteId)
        {
            try
            {
                await _clienteRepository.DeleteClienteAsync(clienteId);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al eliminar el cliente con ClienteId {clienteId}: {ex.Message}", 500);
            }
        }

    }

}


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
            var clientes = await _clienteRepository.GetAllClientesAsync();
            return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        }

        public async Task<ClienteDto> GetClienteByIdAsync(string clienteId)
        {
            var cliente = await _clienteRepository.GetClienteByIdAsync(clienteId);
            if (cliente == null)
            {
                throw new ClienteNotFoundException(clienteId); // Excepción personalizada para cliente no encontrado
            }

            return _mapper.Map<ClienteDto>(cliente);
        }

        public async Task<Cliente> AddClienteAsync(ClienteCreateDto clienteDto)
        {
            if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));

            var cliente = _mapper.Map<Cliente>(clienteDto);
            if (string.IsNullOrEmpty(cliente.ClienteId))
            {
                cliente.ClienteId = Guid.NewGuid().ToString();
            }

            await _clienteRepository.AddClienteAsync(cliente);

            var clienteInfo = _mapper.Map<ClienteInfoDto>(cliente);
            _clientePublisher.PublishCliente(clienteInfo);

            return cliente;
        }

        public async Task UpdateClienteAsync(string clienteId, ClienteUpdateDto clienteDto)
        {
            if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));

            var cliente = await _clienteRepository.GetClienteByIdAsync(clienteId);
            if (cliente == null)
            {
                throw new ClienteNotFoundException($"El cliente con ID {clienteId} no fue encontrado.");
            }

            _mapper.Map(clienteDto, cliente);
            await _clienteRepository.UpdateClienteAsync(cliente);

            var clienteInfo = _mapper.Map<ClienteInfoDto>(cliente);
            _clientePublisher.PublishCliente(clienteInfo);
        }

        public async Task DeleteClienteAsync(string clienteId)
        {
            await _clienteRepository.DeleteClienteAsync(clienteId);
        }

    }

}


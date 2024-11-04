using AutoMapper;
using ClienteService.Application.Exceptions;
using ClienteService.Domain.Entities;
using ClienteService.Domain.Events;
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
                throw new ClienteNotFoundException(clienteId);
            }

            return _mapper.Map<ClienteDto>(cliente);
        }

        public async Task<ClienteCreatedDto> AddClienteAsync(ClienteCreateDto clienteDto)
        {
            if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));

            var cliente = _mapper.Map<Cliente>(clienteDto);
            if (string.IsNullOrEmpty(cliente.ClienteId))
            {
                cliente.ClienteId = Guid.NewGuid().ToString();
            }

            await _clienteRepository.AddClienteAsync(cliente);

            // Crear el evento de cliente creado y publicarlo
            var clienteEvent = new ClienteCreatedEvent
            {
                ClienteId = cliente.ClienteId,
                Nombre = cliente.Nombre,
                Identificacion = cliente.Identificacion
            };
            _clientePublisher.PublishClienteCreated(clienteEvent);


            return _mapper.Map<ClienteCreatedDto>(cliente);
        }


        public async Task UpdateClienteAsync(string clienteId, ClienteUpdateDto clienteDto)
        {
            if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));

            var cliente = await _clienteRepository.GetClienteByIdAsync(clienteId);
            if (cliente == null)
            {
                throw new ClienteNotFoundException($"El cliente con ID {clienteId} no fue encontrado.");
            }

            // Mapear los datos actualizados al cliente existente
            _mapper.Map(clienteDto, cliente);
            await _clienteRepository.UpdateClienteAsync(cliente);

            // Publicar un evento de actualización de cliente
            var clienteEvent = new ClienteUpdatedEvent  
            {
                ClienteId = cliente.ClienteId,
                Nombre = cliente.Nombre,
                Identificacion = cliente.Identificacion
            };

            _clientePublisher.PublishClienteUpdated(clienteEvent);
        }


        public async Task DeleteClienteAsync(string clienteId)
        {
            await _clienteRepository.DeleteClienteAsync(clienteId);
        }
    }
}

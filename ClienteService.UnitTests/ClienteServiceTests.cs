using AutoMapper;
using ClienteService.Application.Exceptions;
using ClienteService.Application.Services;
using ClienteService.Domain.Entities;
using ClienteService.Domain.Interfaces;
using ClienteService.DTOs;
using ClienteService.Producers.RabbitMQ;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteService.UnitTests
{
    public class ClienteServiceTests
    {
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ClientService _clienteService;

        public ClienteServiceTests()
        {
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _mapperMock = new Mock<IMapper>();
            _clienteService = new ClientService(_clienteRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task ObtenerTodosLosClientesAsync_DeberiaRetornarListaDeClientes_CuandoExistenClientes()
        {
            // Arrange
            var clientes = new List<Cliente> { new Cliente { Id = 1, Nombre = "Javier Villarreal", Identificacion = "12345" } };
            _clienteRepositoryMock.Setup(r => r.GetAllClientesAsync()).ReturnsAsync(clientes);
            _mapperMock.Setup(m => m.Map<IEnumerable<ClienteDto>>(clientes)).Returns(new List<ClienteDto> { new ClienteDto { Id = 1, Nombre = "Javier Villarreal", Identificacion = "12345" } });

            // Act
            var result = await _clienteService.GetAllClientesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task ObtenerClientePorIdAsync_DeberiaRetornarCliente_CuandoClienteExiste()
        {
            // Arrange
            var cliente = new Cliente { Id = 1, Nombre = "Javier Villarreal", Identificacion = "12345" };
            _clienteRepositoryMock.Setup(r => r.GetClienteByIdAsync(1)).ReturnsAsync(cliente);
            _mapperMock.Setup(m => m.Map<ClienteDto>(cliente)).Returns(new ClienteDto { Id = 1, Nombre = "Javier Villarreal", Identificacion = "12345" });

            // Act
            var result = await _clienteService.GetClienteByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task ObtenerClientePorIdAsync_DeberiaLanzarExcepcion_CuandoClienteNoExiste()
        {
            // Arrange
            _clienteRepositoryMock.Setup(r => r.GetClienteByIdAsync(1)).ReturnsAsync((Cliente)null);

            // Act & Assert
            await Assert.ThrowsAsync<ClienteNotFoundException>(() => _clienteService.GetClienteByIdAsync(1));
        }

        [Fact]
        public async Task AgregarClienteAsync_DeberiaAgregarCliente_CuandoClienteEsValido()
        {
            // Arrange
            var clienteDto = new ClienteDto { Id = 1, Nombre = "Javier Villarreal", Identificacion = "12345" };
            var cliente = new Cliente { Id = 1, Nombre = "Javier Villarreal", Identificacion = "12345", Estado = "Activo" };

            _mapperMock.Setup(m => m.Map<Cliente>(clienteDto)).Returns(cliente);
            _clienteRepositoryMock.Setup(r => r.AddClienteAsync(cliente)).Returns(Task.CompletedTask);

            // Act
            await _clienteService.AddClienteAsync(clienteDto);

            // Assert
            _clienteRepositoryMock.Verify(r => r.AddClienteAsync(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task ActualizarClienteAsync_DeberiaActualizarCliente_CuandoClienteEsValido()
        {
            // Arrange
            var clienteDto = new ClienteDto { Id = 1, Nombre = "Javier Villarreal", Identificacion = "12345" };
            var cliente = new Cliente { Id = 1, Nombre = "Javier Villarreal", Identificacion = "12345", Estado = "Activo" };

            _mapperMock.Setup(m => m.Map<Cliente>(clienteDto)).Returns(cliente);
            _clienteRepositoryMock.Setup(r => r.UpdateClienteAsync(cliente)).Returns(Task.CompletedTask);

            // Act
            await _clienteService.UpdateClienteAsync(clienteDto);

            // Assert
            _clienteRepositoryMock.Verify(r => r.UpdateClienteAsync(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task EliminarClienteAsync_DeberiaEliminarCliente_CuandoClienteExiste()
        {
            // Arrange
            _clienteRepositoryMock.Setup(r => r.DeleteClienteAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _clienteService.DeleteClienteAsync(1);

            // Assert
            _clienteRepositoryMock.Verify(r => r.DeleteClienteAsync(1), Times.Once);
        }
    }

}

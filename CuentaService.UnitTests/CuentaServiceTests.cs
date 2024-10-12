using AutoMapper;
using CuentaService.Application.Exceptions;
using CuentaService.Application.Services;
using CuentaService.Domain.Entities;
using CuentaService.Domain.Interfaces;
using CuentaService.DTOs;
using Moq;

namespace CuentaService.UnitTests;

public class CuentaServiceTests
{
    private readonly Mock<ICuentaRepository> _cuentaRepositoryMock;
    private readonly Mock<IMovimientoRepository> _movimientoRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CuentService _cuentaService;

    public CuentaServiceTests()
    {
        _cuentaRepositoryMock = new Mock<ICuentaRepository>();
        _movimientoRepositoryMock = new Mock<IMovimientoRepository>();
        _mapperMock = new Mock<IMapper>();
        _cuentaService = new CuentService(_cuentaRepositoryMock.Object, _movimientoRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task AgregarCuentaAsync_DeberiaAgregarCuenta_CuandoCuentaEsValida()
    {
        // Arrange
        var cuentaDto = new CuentaDto { Id = 1, NumeroCuenta = "123456", TipoCuenta = "Ahorro", SaldoInicial = 1000, Estado = "Activa" };
        var cuenta = new Cuenta { Id = 1, NumeroCuenta = "123456", TipoCuenta = "Ahorro", SaldoInicial = 1000, Estado = "Activa" };

        _mapperMock.Setup(m => m.Map<Cuenta>(cuentaDto)).Returns(cuenta);
        _cuentaRepositoryMock.Setup(r => r.AddCuentaAsync(cuenta)).Returns(Task.CompletedTask);

        // Act
        await _cuentaService.AddCuentaAsync(cuentaDto);

        // Assert
        _cuentaRepositoryMock.Verify(r => r.AddCuentaAsync(It.IsAny<Cuenta>()), Times.Once);
    }

    [Fact]
    public async Task AgregarCuentaAsync_DeberiaLanzarExcepcion_CuandoCuentaEsInvalida()
    {
        // Arrange
        CuentaDto cuentaDto = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _cuentaService.AddCuentaAsync(cuentaDto));
    }

    [Fact]
    public async Task ObtenerCuentaPorIdAsync_DeberiaRetornarCuenta_CuandoCuentaExiste()
    {
        // Arrange
        var cuenta = new Cuenta { Id = 1, NumeroCuenta = "123456", TipoCuenta = "Ahorro", SaldoInicial = 1000, Estado = "Activa" };
        _cuentaRepositoryMock.Setup(r => r.GetCuentaByIdAsync(1)).ReturnsAsync(cuenta);
        _mapperMock.Setup(m => m.Map<CuentaDto>(cuenta)).Returns(new CuentaDto { Id = 1, NumeroCuenta = "123456", TipoCuenta = "Ahorro", SaldoInicial = 1000, Estado = "Activa" });

        // Act
        var result = await _cuentaService.GetCuentaByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task ObtenerCuentaPorIdAsync_DeberiaLanzarExcepcion_CuandoCuentaNoExiste()
    {
        // Arrange
        _cuentaRepositoryMock.Setup(r => r.GetCuentaByIdAsync(1)).ReturnsAsync((Cuenta)null);

        // Act & Assert
        await Assert.ThrowsAsync<CuentaNotFoundException>(() => _cuentaService.GetCuentaByIdAsync(1));
    }

    [Fact]
    public async Task ActualizarCuentaAsync_DeberiaActualizarCuenta_CuandoCuentaEsValida()
    {
        // Arrange
        var cuentaDto = new CuentaDto { Id = 1, NumeroCuenta = "123456", TipoCuenta = "Ahorro", SaldoInicial = 1000, Estado = "Activa" };
        var cuenta = new Cuenta { Id = 1, NumeroCuenta = "123456", TipoCuenta = "Ahorro", SaldoInicial = 1000, Estado = "Activa" };

        _mapperMock.Setup(m => m.Map<Cuenta>(cuentaDto)).Returns(cuenta);
        _cuentaRepositoryMock.Setup(r => r.UpdateCuentaAsync(cuenta)).Returns(Task.CompletedTask);

        // Act
        await _cuentaService.UpdateCuentaAsync(cuentaDto);

        // Assert
        _cuentaRepositoryMock.Verify(r => r.UpdateCuentaAsync(It.IsAny<Cuenta>()), Times.Once);
    }

    [Fact]
    public async Task EliminarCuentaAsync_DeberiaEliminarCuenta_CuandoCuentaExiste()
    {
        // Arrange
        var cuenta = new Cuenta { Id = 1, NumeroCuenta = "123456", TipoCuenta = "Ahorro", SaldoInicial = 1000, Estado = "Activa" };
        _cuentaRepositoryMock.Setup(r => r.GetCuentaByIdAsync(1)).ReturnsAsync(cuenta);
        _cuentaRepositoryMock.Setup(r => r.DeleteCuentaAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _cuentaService.DeleteCuentaAsync(1);

        // Assert
        _cuentaRepositoryMock.Verify(r => r.DeleteCuentaAsync(1), Times.Once);
    }

    [Fact]
    public async Task ObtenerMovimientosPorCuentaIdAsync_DeberiaRetornarMovimientos_CuandoCuentaExiste()
    {
        // Arrange
        var movimientos = new List<Movimiento> { new Movimiento { Id = 1, CuentaId = 1, Fecha = DateTime.Now, TipoMovimiento = "Deposito", Valor = 1000, Saldo = 2000 } };
        _movimientoRepositoryMock.Setup(r => r.GetMovimientosByCuentaIdAsync(1)).ReturnsAsync(movimientos);
        _mapperMock.Setup(m => m.Map<IEnumerable<MovimientoDto>>(movimientos)).Returns(new List<MovimientoDto> { new MovimientoDto { Id = 1, CuentaId = 1, TipoMovimiento = "Deposito", Valor = 1000, Saldo = 2000 } });

        // Act
        var result = await _cuentaService.GetMovimientosByCuentaIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}
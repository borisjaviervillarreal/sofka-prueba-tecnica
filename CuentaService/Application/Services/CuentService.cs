using AutoMapper;
using CuentaService.Application.Exceptions;
using CuentaService.Domain.Entities;
using CuentaService.Domain.Interfaces;
using CuentaService.DTOs;
using CuentaService.DTOs.CuentaService.DTOs;
using CuentaService.Producers.RabbitMQ;

namespace CuentaService.Application.Services
{
    public class CuentService : ICuentaService
    {
        private readonly ICuentaRepository _cuentaRepository;
        private readonly IMovimientoRepository _movimientoRepository;
        private readonly IMapper _mapper;

        public CuentService(ICuentaRepository cuentaRepository, IMovimientoRepository movimientoRepository, IMapper mapper)
        {
            _cuentaRepository = cuentaRepository;
            _movimientoRepository = movimientoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CuentaDto>> GetAllCuentasAsync()
        {
            try
            {
                var cuentas = await _cuentaRepository.GetAllCuentasAsync();
                return _mapper.Map<IEnumerable<CuentaDto>>(cuentas);
            }
            catch (Exception ex)
            {
                throw new AppException("Error al obtener la lista de cuentas.", 500);
            }
        }

        public async Task<CuentaDto> GetCuentaByIdAsync(int id)
        {
            try
            {
                var cuenta = await _cuentaRepository.GetCuentaByIdAsync(id);
                if (cuenta == null)
                {
                    throw new CuentaNotFoundException(id);
                }

                return _mapper.Map<CuentaDto>(cuenta);
            }
            catch (CuentaNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener la cuenta con ID {id}: {ex.Message}", 500);
            }
        }


        public async Task AddCuentaAsync(CuentaDto cuentaDto)
        {
            if (cuentaDto == null)
            {
                throw new ArgumentNullException(nameof(cuentaDto), "El objeto cuenta no puede ser nulo.");
            }

            try
            {
                var cuenta = _mapper.Map<Cuenta>(cuentaDto);
                Console.WriteLine($"ClienteId antes de guardar la cuenta: {cuenta.ClienteId}");
                await _cuentaRepository.AddCuentaAsync(cuenta);
            }
            catch (Exception ex)
            {
                throw new AppException("Error al agregar la cuenta.", 500);
            }
        }


        public async Task UpdateCuentaAsync(CuentaDto cuentaDto)
        {
            try
            {
                if (cuentaDto == null)
                {
                    throw new ArgumentNullException(nameof(cuentaDto), "El objeto cuenta no puede ser nulo.");
                }

                var cuenta = _mapper.Map<Cuenta>(cuentaDto);
                await _cuentaRepository.UpdateCuentaAsync(cuenta);
            }
            catch (Exception ex)
            {
                throw new AppException("Error al actualizar la cuenta.", 500);
            }
        }

        public async Task DeleteCuentaAsync(int id)
        {
            try
            {
                var cuenta = await _cuentaRepository.GetCuentaByIdAsync(id);
                if (cuenta == null)
                {
                    throw new CuentaNotFoundException(id);
                }

                await _cuentaRepository.DeleteCuentaAsync(id);
            }
            catch (CuentaNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException("Error al eliminar la cuenta.", 500);
            }
        }
        public async Task<IEnumerable<MovimientoDto>> GetMovimientosByCuentaIdAsync(int cuentaId)
        {
            var movimientos = await _movimientoRepository.GetMovimientosByCuentaIdAsync(cuentaId);
            return _mapper.Map<IEnumerable<MovimientoDto>>(movimientos);
        }

        public async Task AddMovimientoAsync(MovimientoDto movimientoDto)
        {
            try
            {
                // Validar si la cuenta existe
                var cuenta = await _cuentaRepository.GetCuentaByIdAsync(movimientoDto.CuentaId);
                if (cuenta == null)
                {
                    throw new CuentaNotFoundException(movimientoDto.CuentaId);
                }

                // Validar el saldo disponible
                if (movimientoDto.TipoMovimiento == "Retiro" && cuenta.SaldoInicial < movimientoDto.Valor)
                {
                    throw new SaldoInsuficienteException(cuenta.SaldoInicial, movimientoDto.Valor);
                }

                // Actualizar el saldo
                cuenta.SaldoInicial += movimientoDto.TipoMovimiento == "Retiro" ? -movimientoDto.Valor : movimientoDto.Valor;
                await _cuentaRepository.UpdateCuentaAsync(cuenta);

                // Agregar el movimiento
                var movimiento = _mapper.Map<Movimiento>(movimientoDto);
                movimiento.Saldo = cuenta.SaldoInicial;
                movimiento.Fecha = DateTime.UtcNow;

                Console.WriteLine($"Fecha del movimiento antes de insertar: {movimiento.Fecha}");
                await _movimientoRepository.AddMovimientoAsync(movimiento);
            }
            catch (Exception ex)
            {
                throw new AppException("Error al agregar el movimiento.", 500);
            }
        }

        public async Task<List<EstadoCuentaDto>> GetEstadoCuentaAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                // Obtener todas las cuentas del cliente
                var cuentasCliente = await _cuentaRepository.GetAllCuentasAsync();

                var cuentas = cuentasCliente.Where(c => c.ClienteId == clienteId).ToList();


                if (!cuentas.Any())
                {

                    throw new Exception("El cliente no tiene cuentas registradas.");
                }

                // Obtener la información del cliente desde el diccionario
                if (!ClienteCreatedConsumer.ClientesInfo.TryGetValue(clienteId, out var clienteInfo))
                {

                    throw new Exception("No se pudo obtener la información del cliente.");
                }

                var estadoCuentaList = new List<EstadoCuentaDto>();

                foreach (var cuenta in cuentas)
                {


                    // Obtener los movimientos de la cuenta
                    var movimientos = await _movimientoRepository.GetMovimientosByCuentaIdAsync(cuenta.Id);
                    var movimientosFiltrados = movimientos.Where(m => m.Fecha >= fechaInicio && m.Fecha <= fechaFin).ToList();



                    foreach (var movimiento in movimientosFiltrados)
                    {
                        var estadoCuenta = new EstadoCuentaDto
                        {
                            Fecha = movimiento.Fecha.ToString("dd/MM/yyyy"),
                            Cliente = clienteInfo.Nombre,
                            Identificacion = clienteInfo.Identificacion,
                            NumeroCuenta = cuenta.NumeroCuenta,
                            Tipo = cuenta.TipoCuenta,
                            SaldoInicial = cuenta.SaldoInicial,
                            Estado = cuenta.Estado == "Activa",
                            Movimiento = movimiento.Valor,
                            SaldoDisponible = movimiento.Saldo
                        };

                        estadoCuentaList.Add(estadoCuenta);
                    }
                }


                return estadoCuentaList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


    }
}


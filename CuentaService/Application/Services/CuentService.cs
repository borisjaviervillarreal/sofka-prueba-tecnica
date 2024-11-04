using AutoMapper;
using CuentaService.Application.Exceptions;
using CuentaService.Domain.Entities;
using CuentaService.Domain.Interfaces;
using CuentaService.DTOs;
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
            var cuentas = await _cuentaRepository.GetAllCuentasAsync();
            return _mapper.Map<IEnumerable<CuentaDto>>(cuentas);
        }

        public async Task<CuentaDto> GetCuentaByIdAsync(int id)
        {
            var cuenta = await _cuentaRepository.GetCuentaByIdAsync(id);
            if (cuenta == null)
            {
                throw new CuentaNotFoundException(id);
            }

            return _mapper.Map<CuentaDto>(cuenta);
        }

        public async Task<CuentaDto> AddCuentaAsync(CuentaCreateDto cuentaDto)
        {
            if (cuentaDto == null) throw new ArgumentNullException(nameof(cuentaDto), "El objeto cuenta no puede ser nulo.");

            var cuenta = _mapper.Map<Cuenta>(cuentaDto);
            await _cuentaRepository.AddCuentaAsync(cuenta);

            return _mapper.Map<CuentaDto>(cuenta);
        }

        public async Task UpdateCuentaAsync(int id, CuentaUpdateDto cuentaUpdateDto)
        {
            if (cuentaUpdateDto == null) throw new ArgumentNullException(nameof(cuentaUpdateDto), "El objeto cuenta no puede ser nulo.");

            var cuentaExistente = await _cuentaRepository.GetCuentaByIdAsync(id);
            if (cuentaExistente == null)
            {
                throw new CuentaNotFoundException(id);
            }

            _mapper.Map(cuentaUpdateDto, cuentaExistente);
            await _cuentaRepository.UpdateCuentaAsync(cuentaExistente);
        }

        public async Task DeleteCuentaAsync(int id)
        {
            var cuenta = await _cuentaRepository.GetCuentaByIdAsync(id);
            if (cuenta == null)
            {
                throw new CuentaNotFoundException(id);
            }

            await _cuentaRepository.DeleteCuentaAsync(id);
        }

        public async Task<IEnumerable<MovimientoDto>> GetMovimientosByCuentaIdAsync(int cuentaId)
        {
            var movimientos = await _movimientoRepository.GetMovimientosByCuentaIdAsync(cuentaId);
            return _mapper.Map<IEnumerable<MovimientoDto>>(movimientos);
        }

        public async Task AddMovimientoAsync(int cuentaId, MovimientoCreateDto movimientoDto)
        {
            var cuenta = await _cuentaRepository.GetCuentaByIdAsync(cuentaId);
            if (cuenta == null)
            {
                throw new CuentaNotFoundException(cuentaId);
            }

            if (movimientoDto.TipoMovimiento == "Retiro" && cuenta.SaldoInicial < movimientoDto.Valor)
            {
                throw new SaldoInsuficienteException(cuenta.SaldoInicial, movimientoDto.Valor);
            }

            cuenta.SaldoInicial += movimientoDto.TipoMovimiento == "Retiro" ? -movimientoDto.Valor : movimientoDto.Valor;
            await _cuentaRepository.UpdateCuentaAsync(cuenta);

            var movimiento = new Movimiento
            {
                TipoMovimiento = movimientoDto.TipoMovimiento,
                Valor = movimientoDto.Valor,
                Saldo = cuenta.SaldoInicial,
                Fecha = DateTime.UtcNow,
                CuentaId = cuenta.Id
            };

            await _movimientoRepository.AddMovimientoAsync(movimiento);
        }

        public async Task<List<EstadoCuentaDto>> GetEstadoCuentaAsync(string clienteId, DateTime fechaInicio, DateTime fechaFin)
        {
            var cuentasCliente = await _cuentaRepository.GetAllCuentasAsync();
            var cuentas = cuentasCliente.Where(c => c.ClienteId == clienteId).ToList();

            if (!cuentas.Any())
            {
                throw new Exception("El cliente no tiene cuentas registradas.");
            }

            if (!ClienteCreatedConsumer.ClientesInfo.TryGetValue(clienteId, out var clienteInfo))
            {
                throw new Exception("No se pudo obtener la información del cliente.");
            }

            var estadoCuentaList = new List<EstadoCuentaDto>();

            foreach (var cuenta in cuentas)
            {
                var movimientos = await _movimientoRepository.GetMovimientosByCuentaIdAsync(cuenta.Id);
                var movimientosFiltrados = movimientos.Where(m => m.Fecha >= fechaInicio && m.Fecha <= fechaFin).ToList();

                foreach (var movimiento in movimientosFiltrados)
                {
                    estadoCuentaList.Add(new EstadoCuentaDto
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
                    });
                }
            }

            return estadoCuentaList;
        }
    }
}

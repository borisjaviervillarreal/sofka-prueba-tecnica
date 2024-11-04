using CuentaService.Application.Exceptions;
using CuentaService.Application.Services;
using CuentaService.DTOs;
using CuentaService.DTOs.CuentaService.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CuentaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        private readonly ICuentaService _cuentaService;

        public CuentaController(ICuentaService cuentaService)
        {
            _cuentaService = cuentaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuentaDto>>> GetAllCuentas()
        {
            var cuentas = await _cuentaService.GetAllCuentasAsync();
            return Ok(cuentas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CuentaDto>> GetCuentaById(int id)
        {
            var cuenta = await _cuentaService.GetCuentaByIdAsync(id);
            return Ok(cuenta);
        }

        [HttpPost]
        public async Task<ActionResult> AddCuenta([FromBody] CuentaCreateDto cuentaDto)
        {
            var cuentaCreada = await _cuentaService.AddCuentaAsync(cuentaDto);

            return CreatedAtAction(nameof(GetCuentaById), new { id = cuentaCreada.Id }, cuentaCreada);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCuenta(int id, [FromBody] CuentaUpdateDto cuentaUpdateDto)
        {
            await _cuentaService.UpdateCuentaAsync(id, cuentaUpdateDto);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCuenta(int id)
        {
            await _cuentaService.DeleteCuentaAsync(id);
            return NoContent();
        }

        [HttpGet("{cuentaId}/movimientos")]
        public async Task<ActionResult<IEnumerable<MovimientoDto>>> GetMovimientosByCuentaId(int cuentaId)
        {
            var movimientos = await _cuentaService.GetMovimientosByCuentaIdAsync(cuentaId);
            return Ok(movimientos);
        }

        [HttpPost("{cuentaId}/movimientos")]
        public async Task<ActionResult> AddMovimiento(int cuentaId, [FromBody] MovimientoCreateDto movimientoDto)
        {
            // Como el cuentaId viene por parámetro, no necesitamos asignarlo al DTO
            await _cuentaService.AddMovimientoAsync(cuentaId, movimientoDto);
            return Ok(new { message = "Movimiento agregado exitosamente" });
        }

        [HttpGet("reporte")]
        public async Task<ActionResult<List<EstadoCuentaDto>>> GetEstadoCuenta([FromQuery] string clienteId, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            var estadoCuenta = await _cuentaService.GetEstadoCuentaAsync(clienteId, fechaInicio, fechaFin);
            return Ok(estadoCuenta);
        }
    }
}

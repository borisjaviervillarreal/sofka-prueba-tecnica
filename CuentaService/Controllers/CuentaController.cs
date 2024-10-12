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
            try
            {
                var cuentas = await _cuentaService.GetAllCuentasAsync();
                return Ok(cuentas);
            }
            catch (AppException ex)
            {
                return StatusCode(ex.StatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ha ocurrido un error inesperado." });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CuentaDto>> GetCuentaById(int id)
        {
            try
            {
                var cuenta = await _cuentaService.GetCuentaByIdAsync(id);
                return Ok(cuenta);
            }
            catch (CuentaNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (AppException ex)
            {
                return StatusCode(ex.StatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ha ocurrido un error inesperado." });
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddCuenta([FromBody] CuentaCreateDto cuentaDto)
        {
            try
            {
                var cuentaCreada = await _cuentaService.AddCuentaAsync(cuentaDto);

                return CreatedAtAction(nameof(GetCuentaById), new { id = cuentaCreada.Id }, cuentaCreada);
            }
            catch (AppException ex)
            {
                return StatusCode(ex.StatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ha ocurrido un error inesperado." });
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCuenta(int id, [FromBody] CuentaUpdateDto cuentaUpdateDto)
        {
            try
            {
                await _cuentaService.UpdateCuentaAsync(id, cuentaUpdateDto);
                return NoContent();
            }
            catch (CuentaNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (AppException ex)
            {
                return StatusCode(ex.StatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ha ocurrido un error inesperado." });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCuenta(int id)
        {
            try
            {
                await _cuentaService.DeleteCuentaAsync(id);
                return NoContent();
            }
            catch (CuentaNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (AppException ex)
            {
                return StatusCode(ex.StatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ha ocurrido un error inesperado." });
            }
        }

        [HttpGet("{cuentaId}/movimientos")]
        public async Task<ActionResult<IEnumerable<MovimientoDto>>> GetMovimientosByCuentaId(int cuentaId)
        {
            try
            {
                var movimientos = await _cuentaService.GetMovimientosByCuentaIdAsync(cuentaId);
                return Ok(movimientos);
            }
            catch (CuentaNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (AppException ex)
            {
                return StatusCode(ex.StatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ha ocurrido un error inesperado." });
            }
        }

        [HttpPost("{cuentaId}/movimientos")]
        public async Task<ActionResult> AddMovimiento(int cuentaId, [FromBody] MovimientoCreateDto movimientoDto)
        {
            try
            {
                // Como el cuentaId viene por parámetro, no necesitamos asignarlo al DTO
                await _cuentaService.AddMovimientoAsync(cuentaId, movimientoDto);
                return Ok(new { message = "Movimiento agregado exitosamente" });
            }
            catch (SaldoInsuficienteException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (CuentaNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (AppException ex)
            {
                return StatusCode(ex.StatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ha ocurrido un error inesperado." });
            }
        }



        [HttpGet("reporte")]
        public async Task<ActionResult<List<EstadoCuentaDto>>> GetEstadoCuenta([FromQuery] string clienteId, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var estadoCuenta = await _cuentaService.GetEstadoCuentaAsync(clienteId, fechaInicio, fechaFin);
                return Ok(estadoCuenta);
            }
            catch (CuentaNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (AppException ex)
            {
                return StatusCode(ex.StatusCode, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ha ocurrido un error inesperado." });
            }
        }
    }
}

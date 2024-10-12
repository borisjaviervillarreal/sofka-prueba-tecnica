using ClienteService.Application.Exceptions;
using ClienteService.Application.Services;
using ClienteService.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClienteService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAllClientes()
        {
            try
            {
                var clientes = await _clienteService.GetAllClientesAsync();
                return Ok(clientes);
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
        public async Task<ActionResult<ClienteDto>> GetClienteById(int id)
        {
            try
            {
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                return Ok(cliente);
            }
            catch (ClienteNotFoundException ex)
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
        public async Task<ActionResult> AddCliente([FromBody] ClienteDto clienteDto)
        {
            try
            {
                await _clienteService.AddClienteAsync(clienteDto);
                return CreatedAtAction(nameof(GetClienteById), new { id = clienteDto.Id }, clienteDto);
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
        public async Task<ActionResult> UpdateCliente(int id, [FromBody] ClienteDto clienteDto)
        {
            if (id != clienteDto.Id)
            {
                return BadRequest(new { error = "El ID proporcionado no coincide con el ID del cliente." });
            }

            try
            {
                await _clienteService.UpdateClienteAsync(clienteDto);
                return NoContent();
            }
            catch (ClienteNotFoundException ex)
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
        public async Task<ActionResult> DeleteCliente(int id)
        {
            try
            {
                await _clienteService.DeleteClienteAsync(id);
                return NoContent();
            }
            catch (ClienteNotFoundException ex)
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

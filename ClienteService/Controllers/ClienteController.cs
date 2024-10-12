using ClienteService.Application.Exceptions;
using ClienteService.Application.Services;
using ClienteService.Domain.Entities;
using ClienteService.DTOs;
using CuentaService.DTOs;
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

        [HttpGet("{clienteId}")]
        public async Task<ActionResult<ClienteDto>> GetClienteById(string clienteId)
        {
            try
            {
                var cliente = await _clienteService.GetClienteByIdAsync(clienteId);
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
        public async Task<ActionResult> AddCliente([FromBody] ClienteCreateDto clienteDto)
        {
            try
            {
                var cliente = await _clienteService.AddClienteAsync(clienteDto);
                return CreatedAtAction(nameof(GetClienteById), new { clienteId = cliente.ClienteId }, cliente);
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

        [HttpPut("{clienteId}")]
        public async Task<ActionResult> UpdateCliente(string clienteId, [FromBody] ClienteUpdateDto clienteDto)
        {
            try
            {
                await _clienteService.UpdateClienteAsync(clienteId, clienteDto);
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


        [HttpDelete("{clienteId}")]
        public async Task<ActionResult> DeleteCliente(string clienteId)
        {
            try
            {
                await _clienteService.DeleteClienteAsync(clienteId);
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

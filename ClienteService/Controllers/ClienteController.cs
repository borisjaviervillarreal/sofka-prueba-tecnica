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
            var clientes = await _clienteService.GetAllClientesAsync();
            return Ok(clientes);
        }

        [HttpGet("{clienteId}")]
        public async Task<ActionResult<ClienteDto>> GetClienteById(string clienteId)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(clienteId);
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult> AddCliente([FromBody] ClienteCreateDto clienteDto)
        {
            var cliente = await _clienteService.AddClienteAsync(clienteDto);
            return CreatedAtAction(nameof(GetClienteById), new { clienteId = cliente.ClienteId }, cliente);
        }

        [HttpPut("{clienteId}")]
        public async Task<ActionResult> UpdateCliente(string clienteId, [FromBody] ClienteUpdateDto clienteDto)
        {
            await _clienteService.UpdateClienteAsync(clienteId, clienteDto);
            return NoContent();
        }

        [HttpDelete("{clienteId}")]
        public async Task<ActionResult> DeleteCliente(string clienteId)
        {
            await _clienteService.DeleteClienteAsync(clienteId);
            return NoContent();
        }

    }
}

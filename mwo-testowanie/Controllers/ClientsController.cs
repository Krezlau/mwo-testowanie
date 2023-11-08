using Microsoft.AspNetCore.Mvc;
using mwo_testowanie.Models.DTOs;
using mwo_testowanie.Services;

namespace mwo_testowanie.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : Controller
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        try
        {
            return Ok(await _clientService.GetAllClientsAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClient(Guid id)
    {
        try
        {
            return Ok(await _clientService.GetClientAsync(id));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateClient(ClientCreateDTO client)
    {
        try
        {
            return Ok(await _clientService.CreateClientAsync(client));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateClient(Guid id, ClientCreateDTO client)
    {
        try
        {
            await _clientService.UpdateClientAsync(id, client);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteClient(Guid id)
    {
        try
        {
            await _clientService.DeleteClientAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
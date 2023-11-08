using Microsoft.AspNetCore.Mvc;
using mwo_testowanie.Models;
using mwo_testowanie.Models.DTOs;
using mwo_testowanie.Services;

namespace mwo_testowanie.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserOrders([FromQuery] Guid userId)
    {
        try
        {
            return Ok(await _orderService.GetOrdersForUserAsync(userId));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        try
        {
            return Ok(await _orderService.GetOrderAsync(id));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderCreateDTO order)
    {
        try
        {
            return Ok(await _orderService.CreateOrderAsync(order));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOrderState(Guid id, OrderState state)
    {
        try
        {
            await _orderService.UpdateOrderStateAsync(id, state);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        try
        {
            await _orderService.CancelOrderAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
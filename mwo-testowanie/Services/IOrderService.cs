using mwo_testowanie.Models;
using mwo_testowanie.Models.DTOs;

namespace mwo_testowanie.Services;

public interface IOrderService
{
    Task<OrderDTO> GetOrderAsync(Guid id);
    Task<List<OrderDTO>> GetOrdersForUserAsync(Guid userId);
    Task<Guid> CreateOrderAsync(OrderCreateDTO order);
    Task UpdateOrderStateAsync(Guid id, OrderState state);
    Task CancelOrderAsync(Guid id);
}
using AutoMapper;
using mwo_testowanie.Models;
using mwo_testowanie.Models.DTOs;
using mwo_testowanie.Repository;

namespace mwo_testowanie.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _repository;
    private readonly IMapper _mapper;
    private readonly IRepository<Product> _productRepo;

    public OrderService(IRepository<Order> repository, IMapper mapper, IRepository<Product> productRepo)
    {
        _repository = repository;
        _mapper = mapper;
        _productRepo = productRepo;
    }

    public async Task<OrderDTO> GetOrderAsync(Guid id)
    {
        return _mapper.Map<OrderDTO>(await _repository.GetAsync(o => o.Id == id));
    }

    public async Task<List<OrderDTO>> GetOrdersForUserAsync(Guid userId)
    {
        return _mapper.Map<List<OrderDTO>>(await _repository.GetAllAsync(o => o.UserId == userId));
    }

    public async Task<Guid> CreateOrderAsync(OrderCreateDTO order)
    {
        if (order is null) throw new ArgumentNullException(nameof(order));
        
        var orderEntity = _mapper.Map<Order>(order);
        orderEntity.Products = new List<(Product, int)>();
        foreach (var (product, quantity) in order.ProductIds)
        {
            var productEntity = await _productRepo.GetAsync(p => p.Id == product);
            if (productEntity is null) throw new ArgumentException($"Product with id {product} does not exist");
            
            // update product quantity
            productEntity.QuantityLeft -= quantity;
            await _productRepo.UpdateAsync(productEntity);
            
            orderEntity.Products.Add((productEntity, quantity));
        }

        await _repository.CreateAsync(orderEntity);
        return orderEntity.Id;
    }

    public async Task UpdateOrderStateAsync(Guid id, OrderState state)
    {
        var orderEntity = await _repository.GetAsync(o => o.Id == id);
        if (orderEntity is null) throw new ArgumentException($"Order with id {id} does not exist");
        
        orderEntity.State = state;
        await _repository.UpdateAsync(orderEntity);
    }

    public async Task CancelOrderAsync(Guid id)
    {
        var orderEntity = await _repository.GetAsync(o => o.Id == id);
        if (orderEntity is null) throw new ArgumentException($"Order with id {id} does not exist");
        
        // update product quantity
        foreach (var (product, quantity) in orderEntity.Products)
        {
            product.QuantityLeft += quantity;
            await _productRepo.UpdateAsync(product);
        }

        await _repository.DeleteAsync(orderEntity);
    }
}
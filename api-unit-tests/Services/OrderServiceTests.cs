using System.Collections;
using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using mwo_testowanie;
using mwo_testowanie.Models;
using mwo_testowanie.Models.DTOs;
using mwo_testowanie.Repository;
using mwo_testowanie.Services;

namespace api_unit_tests.Services;

public class OrderServiceTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IRepository<Order>> _orderRepoMock = new();
    private readonly Mock<IRepository<Product>> _productRepoMock = new();
    private readonly Mock<IRepository<Client>> _clientRepoMock = new();

    public OrderServiceTests()
    {
        var myProfile = new AutoMapperProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        _mapper = new Mapper(configuration);
    }

    public static IEnumerable<object[]> OrderData => new List<object[]>()
    {
        new object[] {new Order() { Client = null, Id = new Guid(), Products = null, State = OrderState.New, UserId = new Guid() }},
        new object[] {null}
    };

    [Theory]
    [MemberData(nameof(OrderData))]
    public async Task GetOrderAsync_ShouldReturnOrder(Order order)
    {
        // Arrange
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(order);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        var result = await service.GetOrderAsync(new Guid());
        
        // Assert
        result.Should().Be(_mapper.Map<OrderDTO>(order));
    }
    
    [Fact]
    public async Task GetOrderAsync_OrderDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync((Order)null);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        var result = await service.GetOrderAsync(new Guid());
        
        // Assert
        _orderRepoMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
            It.IsAny<string>()), Times.Once);
        result.Should().Be(null);
    }
    
    [Theory]
    [MemberData(nameof(OrderData))]
    public async Task GetOrdersForUserAsync_ShouldReturnOrders(Order order)
    {
        // Arrange
        _orderRepoMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<Order>() {order});
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        var result = await service.GetOrdersForUserAsync(new Guid());
        
        // Assert
        result.Should().BeEquivalentTo(_mapper.Map<List<OrderDTO>>(new List<Order>() {order}));
    }
    
    [Fact]
    public async Task GetOrdersForUserAsync_ShouldReturnEmptyList_WhenUserHasNoOrders()
    {
        // Arrange
        _orderRepoMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(new List<Order>());
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        var result = await service.GetOrdersForUserAsync(new Guid());
        
        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetOrdersForUserAsync_ShouldReturnEmptyList_WhenUserDoesNotExist()
    {
        // Arrange
        _orderRepoMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync((List<Order>)null);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        var result = await service.GetOrdersForUserAsync(new Guid());
        
        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenOrderIsNull()
    {
        // Arrange
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        Func<Task> act = async () => await service.CreateOrderAsync(null);
        
        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
    
    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenClientDoesNotExist()
    {
        // Arrange
        _clientRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Client, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync((Client)null);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        Func<Task> act = async () => await service.CreateOrderAsync(new OrderCreateDTO());
        
        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        _clientRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Client, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(new Client());
        
        _productRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync((Product)null);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        Func<Task> act = async () => await service.CreateOrderAsync(new OrderCreateDTO()
            { ClientId = new Guid(), ProductIds = new List<(Guid, int)>() {(new Guid(), 1)} });
        
        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrder()
    {
        // Arrange
        var client = new Client() {Id = new Guid()};
        var product = new Product() {Id = new Guid(), QuantityLeft = 10};
        var order = new OrderCreateDTO()
        {
            ClientId = client.Id,
            ProductIds = new List<(Guid, int)>() {(product.Id, 1)}
        };
        
        _clientRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Client, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(client);
        
        _productRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(product);

        _orderRepoMock.Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .Callback<Order>((o) =>
            {
                o.Id = new Guid();
            });
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        var result = await service.CreateOrderAsync(order);
        
        // Assert
        _orderRepoMock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
        _productRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
        _clientRepoMock.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Client, bool>>>(),
            It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateOrderAsync_ShouldUpdateProductQuantity()
    {
        // Arrange
        var client = new Client() {Id = new Guid()};
        var product = new Product() {Id = new Guid(), QuantityLeft = 10};
        var order = new OrderCreateDTO()
        {
            ClientId = client.Id,
            ProductIds = new List<(Guid, int)>() {(product.Id, 1)}
        };
        
        _clientRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Client, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(client);
        
        _productRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(product);
        
        _orderRepoMock.Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .Callback<Order>((o) =>
            {
                o.Id = new Guid();
            });
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        var result = await service.CreateOrderAsync(order);
        
        // Assert
        product.QuantityLeft.Should().Be(9);
    }
    
    [Fact]
    public async Task UpdateOrderStateAsync_ShouldThrowException_WhenOrderDoesNotExist()
    {
        // Arrange
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync((Order)null);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        Func<Task> act = async () => await service.UpdateOrderStateAsync(new Guid(), OrderState.New);
        
        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task UpdateOrderStateAsync_ShouldUpdateOrderState()
    {
        // Arrange
        var order = new Order() {Id = new Guid(), State = OrderState.New};
        
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(order);
        
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        var newState = OrderState.InProgress;
        
        // Act
        await service.UpdateOrderStateAsync(order.Id, newState);
        
        // Assert
        order.State.Should().Be(newState);
    }
    
    [Fact]
    public async Task CancelOrderAsync_ShouldThrowException_WhenOrderDoesNotExist()
    {
        // Arrange
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync((Order)null);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        Func<Task> act = async () => await service.CancelOrderAsync(new Guid());
        
        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task CancelOrderAsync_ShouldDeleteOrder()
    {
        // Arrange
        var order = new Order() {Id = new Guid(), State = OrderState.New, Products = new List<ProductQuantity>()};
        
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(order);
        
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CancelOrderAsync(order.Id));
    }

    [Fact]
    public async Task CancelOrderAsync_ProductListIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var order = new Order() {Id = new Guid(), State = OrderState.New };
        
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(order);
        
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        Assert.ThrowsAsync<ArgumentNullException>(async() => await service.CancelOrderAsync(order.Id));
        
    }
    
    [Fact]
    public async Task CancelOrderAsync_ShouldUpdateProductQuantity()
    {
        // Arrange
        var order = new Order() {Id = new Guid(), State = OrderState.New};
        var product = new Product() {Id = new Guid(), QuantityLeft = 10};
        order.Products = new List<ProductQuantity>()
        {
            new ProductQuantity() {Product = product, Quantity = 1}
        };
        
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(order);
        
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        
        _productRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(product);
        
        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        await service.CancelOrderAsync(order.Id);
        
        // Assert
        product.QuantityLeft.Should().Be(11);
    }
    
    [Fact]
    public async Task CancelOrderAsync_ShouldNotUpdateProductQuantity_WhenProductDoesNotExist()
    {
        // Arrange
        var order = new Order() {Id = new Guid(), State = OrderState.New};
        var product = new Product() {Id = new Guid(), QuantityLeft = 10};
        order.Products = new List<ProductQuantity>()
        {
            new ProductQuantity() {Product = product, Quantity = 1}
        };
        
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(order);
        
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        
        _productRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync((Product)null);
        
        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        Assert.ThrowsAsync<ArgumentException>(() => service.CancelOrderAsync(order.Id));
    }
    
    [Fact]
    public async Task CancelOrderAsync_ShouldNotUpdateProductQuantity_WhenProductQuantityIsZero()
    {
        // Arrange
        var order = new Order() {Id = new Guid(), State = OrderState.New};
        var product = new Product() {Id = new Guid(), QuantityLeft = 10};
        order.Products = new List<ProductQuantity>()
        {
            new ProductQuantity() {Product = product, Quantity = 0}
        };
        
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(order);
        
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        
        _productRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(product);
        
        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        await service.CancelOrderAsync(order.Id);
        
        // Assert
        product.QuantityLeft.Should().Be(10);
    }
    
    [Fact]
    public async Task CancelOrderAsync_ShouldNotUpdateProductQuantity_WhenProductQuantityIsNegative()
    {
        // Arrange
        var order = new Order() {Id = new Guid(), State = OrderState.New};
        var product = new Product() {Id = new Guid(), QuantityLeft = 10};
        order.Products = new List<ProductQuantity>()
        {
            new ProductQuantity() {Product = product, Quantity = -1}
        };
        
        _orderRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(order);
        
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        
        _productRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(product);
        
        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        await service.CancelOrderAsync(order.Id);
        
        // Assert
        product.QuantityLeft.Should().Be(10);
    }

    [Fact]
    public async Task UpdateOrderAsync_LastProductIsInvalid_ShouldNotCreateEntityEndNotUpdateQuantities()
    {
        // Arrange
        var product = new Product() {Id = new Guid(), QuantityLeft = 10};
        var product2 = new Product() {Id = new Guid("8f7aaf96-bf12-48bb-8104-cd56086241c1"), QuantityLeft = 10};
        var order = new OrderCreateDTO()
        {
            ClientId = new Guid(),
            ProductIds = new List<(Guid, int)>() {(product.Id, 1), (product2.Id, 2)}
        };
        
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        
        _clientRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Client, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(new Client());

        _productRepoMock.SetupSequence(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(product)
            .ReturnsAsync((Product?)null);
        
        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);
        
        var service = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _clientRepoMock.Object);
        
        // Act
        Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateOrderAsync(order));
        
        // Assert
        product.QuantityLeft.Should().Be(10);
    }
}
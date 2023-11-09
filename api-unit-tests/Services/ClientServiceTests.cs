using System.Collections;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using mwo_testowanie;
using mwo_testowanie.Models;
using mwo_testowanie.Models.DTOs;
using mwo_testowanie.Repository;
using mwo_testowanie.Services;

namespace api_unit_tests.Services;

public class ClientServiceTests
{
    private readonly IMapper _mapper;

    public ClientServiceTests()
    {
        var myProfile = new AutoMapperProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        _mapper = new Mapper(configuration);
    }

    [Fact]
    public void GetAllClientsAsync_WhenClientsExist_ShouldReturnCorrectlyMappedClients()
    {
        // Arrange
        var clients = new List<Client>()
        {
            new Client()
                { Id = new Guid(), Email = "john@doe.com", Name = "John", Surname = "Doe", Orders = new List<Order>() },
            new Client()
            {
                Id = new Guid(), Email = "jackson@doe.com", Name = "Jackson", Surname = "Doe",
                Orders = new List<Order>()
            },
            new Client()
            {
                Id = new Guid(), Email = "michael@doe.com", Name = "Michael", Surname = "Doe",
                Orders = new List<Order>()
            },
        };
        
        var mockRepo = new Mock<IRepository<Client>>();
        mockRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(clients);
        
        var service = new ClientService(mockRepo.Object, _mapper);
        
        // Act
        var result = service.GetAllClientsAsync().Result;
        
        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<ClientDTO>>(result);
        mockRepo.Verify(repo => repo.GetAllAsync(
            It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public void GetAllClientsAsync_NoClients_ShouldReturnEmptyList()
    {
        // Arrange
        var clients = new List<Client>();
        
        var mockRepo = new Mock<IRepository<Client>>();
        mockRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(clients);
        
        var service = new ClientService(mockRepo.Object, _mapper);
        
        // Act
        var result = service.GetAllClientsAsync().Result;
        
        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<ClientDTO>>(result);
        Assert.Empty(result);
        mockRepo.Verify(repo => repo.GetAllAsync(
            It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void GetClientAsync_ClientExist_ShouldCorrectlyCallRepoAndMapper()
    {
        // Arrange
        var client = new Client()
        {
            Id = new Guid(), Email = "john@doe.com", Name = "John", Surname = "Doe", Orders = new List<Order>()
        };
        
        var mockRepo = new Mock<IRepository<Client>>();
        mockRepo.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(client);

        var service = new ClientService(mockRepo.Object, _mapper);
        
        // Act
        var result = service.GetClientAsync(new Guid()).Result;
        
        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<ClientDTO>(result);
        mockRepo.Verify(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public void GetClientAsync_ClientDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Client>>();
        mockRepo.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync((Client)null);
        
        var service = new ClientService(mockRepo.Object, _mapper);
        
        // Act
        var result = service.GetClientAsync(new Guid()).Result;
        
        // Assert
        Assert.Null(result);
        mockRepo.Verify(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void CreateClientAsync_ClientIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Client>>();
        var mockMapper = new Mock<IMapper>();
        var service = new ClientService(mockRepo.Object, mockMapper.Object);
        
        // Act
        var exception = Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateClientAsync(null));
        
        // Assert
        Assert.NotNull(exception);
        mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Client>()), Times.Never);
        mockMapper.Verify(mapper => mapper.Map<Client>(It.IsAny<ClientDTO>()), Times.Never);
    }

    [Fact]
    public void CreateClientAsync_ClientIsValid_ShouldCorrectlyCallRepoAndMapper()
    {
        // Arrange
        var client = new ClientCreateDTO()
        {
            Email = "john@doe.com", Name = "John", Surname = "Doe"
        };
        
        var mockRepo = new Mock<IRepository<Client>>();
        mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Client>())).Callback<Client>(r =>
        {
            r.Id = new Guid();
        });

        var service = new ClientService(mockRepo.Object, _mapper);
        
        // Act
        var result = service.CreateClientAsync(client).Result;
        
        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Guid>(result);
        mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Client>()), Times.Once);
    }
    
    [Fact]
    public void UpdateClientAsync_ClientIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Client>>();
        var mockMapper = new Mock<IMapper>();
        var service = new ClientService(mockRepo.Object, mockMapper.Object);
        
        // Act
        var exception = Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateClientAsync(new Guid(), null));
        
        // Assert
        Assert.NotNull(exception);
        mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Client>()), Times.Never);
        mockMapper.Verify(mapper => mapper.Map<Client>(It.IsAny<ClientDTO>()), Times.Never);
    }

    [Fact]
    public void UpdateClientAsync_ClientDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange
        var client = new ClientCreateDTO()
        {
            Email = "john@doe.com", Name = "John", Surname = "Doe"
        };

        var mockRepo = new Mock<IRepository<Client>>();
        mockRepo.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync((Client)null);

        var service = new ClientService(mockRepo.Object, _mapper);

        // Act
        var exception = Assert.ThrowsAsync<ArgumentException>(() => service.UpdateClientAsync(new Guid(), client));

        // Assert
        Assert.NotNull(exception);
        mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Client>()), Times.Never);
        mockRepo.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async void UpdateClientAsync_ClientIsValid_ShouldCorrectlyCallRepoAndMapper()
    {
        // Arrange
        var client = new ClientCreateDTO()
        {
            Email = "john@doe.com", Name = "John", Surname = "Doe"
        };
        var clientEntity = _mapper.Map<Client>(client);
        clientEntity.Id = new Guid();

        var mockRepo = new Mock<IRepository<Client>>();
        mockRepo.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(clientEntity);
        
        var service = new ClientService(mockRepo.Object, _mapper);
        
        // Act
        await service.UpdateClientAsync(new Guid(), client);
        
        // Assert
        mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Client>()), Times.Once);
        mockRepo.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()),
            Times.Once);
        
    }

    [Fact]
    public async void DeleteClientAsync_ClientInDb_ShouldCorrectlyCallRepo()
    {
        // Arrange
        var client = new Client()
        {
            Id = new Guid(), Email = "john@doe.com", Name = "John", Surname = "Doe", Orders = new List<Order>()
        };

        var mockRepo = new Mock<IRepository<Client>>();
        mockRepo.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(client);

        var service = new ClientService(mockRepo.Object, _mapper);

        // Act
        await service.DeleteClientAsync(new Guid());

        // Assert
        mockRepo.Verify(repo => repo.DeleteAsync(It.IsAny<Client>()), Times.Once);
        mockRepo.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()),
            Times.Once);
    }
    
    [Fact]
    public async void DeleteClientAsync_ClientNotInDb_ShouldThrowException()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Client>>();
        mockRepo.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync((Client)null);

        var service = new ClientService(mockRepo.Object, _mapper);

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => service.DeleteClientAsync(new Guid()));

        // Assert
        Assert.NotNull(exception);
        mockRepo.Verify(repo => repo.DeleteAsync(It.IsAny<Client>()), Times.Never);
        mockRepo.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<Client, bool>>>(), It.IsAny<string>()),
            Times.Once);
    }
}
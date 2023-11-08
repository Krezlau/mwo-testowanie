using AutoMapper;
using mwo_testowanie.Models;
using mwo_testowanie.Models.DTOs;

namespace mwo_testowanie;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Client, ClientDTO>();
        CreateMap<ClientCreateDTO, Client>();
        CreateMap<Order, OrderDTO>();
        CreateMap<OrderCreateDTO, Order>();
        CreateMap<Product, ProductDTO>();
        CreateMap<ProductCreateDTO, Product>();
    }
}
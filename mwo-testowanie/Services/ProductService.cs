using AutoMapper;
using mwo_testowanie.Models;
using mwo_testowanie.Models.DTOs;
using mwo_testowanie.Repository;

namespace mwo_testowanie.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _repository;
    private readonly IMapper _mapper;

    public ProductService(IRepository<Product> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ProductDTO>> GetProductsAsync()
    {
        return _mapper.Map<List<ProductDTO>>(await _repository.GetAllAsync());
    }

    public async Task<ProductDTO> GetProductAsync(Guid id)
    {
        return _mapper.Map<ProductDTO>(await _repository.GetAsync(p => p.Id == id));
    }

    public async Task<Guid> CreateProductAsync(ProductCreateDTO product)
    {
        var productEntity = _mapper.Map<Product>(product);
        await _repository.CreateAsync(productEntity);
        return productEntity.Id;
    }

    public async Task UpdateProductAsync(Guid id, ProductCreateDTO product)
    {
        if (product is null) throw new ArgumentNullException(nameof(product));
        
        var productEntity = await _repository.GetAsync(p => p.Id == id);
        if (productEntity is null) throw new ArgumentException($"Product with id {id} does not exist");
        
        _mapper.Map(product, productEntity);
        await _repository.UpdateAsync(productEntity);
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var productEntity = await _repository.GetAsync(p => p.Id == id);
        if (productEntity is null) throw new ArgumentException($"Product with id {id} does not exist");
        
        await _repository.DeleteAsync(productEntity);
    }
}
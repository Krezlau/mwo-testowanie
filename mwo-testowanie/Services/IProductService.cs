using mwo_testowanie.Models.DTOs;

namespace mwo_testowanie.Services;

public interface IProductService
{
    Task<List<ProductDTO>> GetProductsAsync();
    Task<ProductDTO> GetProductAsync(Guid id);
    Task<Guid> CreateProductAsync(ProductCreateDTO product);
    Task UpdateProductAsync(Guid id, ProductCreateDTO product);
    Task DeleteProductAsync(Guid id);
}
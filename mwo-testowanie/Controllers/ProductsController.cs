using Microsoft.AspNetCore.Mvc;
using mwo_testowanie.Models.DTOs;
using mwo_testowanie.Services;

namespace mwo_testowanie.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            return Ok(await _productService.GetProductsAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        try
        {
            return Ok(await _productService.GetProductAsync(id));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductCreateDTO product)
    {
        try
        {
            return Ok(await _productService.CreateProductAsync(product));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, ProductCreateDTO product)
    {
        try
        {
            await _productService.UpdateProductAsync(id, product);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}